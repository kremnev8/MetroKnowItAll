using System;
using Model;
using Platformer.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Gameplay
{
    public class UISwipe : MonoBehaviour
    {
        private PlayerInput input;

        private InputAction primaryPositionAction;
        private InputAction primaryContactAction;

        private RectTransform rectTransform;
        private Vector2 initialPos;
        private bool isSwiping;
        private bool isReturning;

        public float returnSpeed;
        public Vector2 moveAxis;
        public Vector2 closeThreshold;

        private void Start()
        {
            GameModel model = Simulation.GetModel<GameModel>();
            input = model.input;

            rectTransform = GetComponent<RectTransform>();

            primaryPositionAction = input.actions["primaryPosition"];
            primaryContactAction = input.actions["primaryContact"];
            
            primaryContactAction.started += StartSwipe;
            primaryContactAction.canceled += EndSwipe;
        }

        private void OnEnable()
        {
            if (rectTransform != null)
            {
                isReturning = true;
                rectTransform.anchoredPosition = rectTransform.sizeDelta * moveAxis;
            }
        }

        private void Update()
        {
            if (isSwiping)
            {
                Vector2 delta = GetTransformedPosition() - initialPos;
                if (Vector2.Dot(delta.normalized, moveAxis) > 0)
                {
                    rectTransform.anchoredPosition = delta * moveAxis.Abs();
                }
            }
            else
            {
                if ((rectTransform.anchoredPosition * moveAxis).Greater(closeThreshold) && !isReturning)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, Vector2.zero, returnSpeed * Time.deltaTime);
                    
                    if (rectTransform.anchoredPosition.sqrMagnitude < 1) isReturning = false;
                }
            }


        }

        private Vector2 GetTransformedPosition()
        {
            return transform.parent.InverseTransformPoint(primaryPositionAction.ReadValue<Vector2>());
        }
        

        private void StartSwipe(InputAction.CallbackContext obj)
        {
            if (gameObject.activeSelf)
            {
                
                initialPos = GetTransformedPosition();
                isSwiping = true;
                isReturning = false;
            }
        }
        
        private void EndSwipe(InputAction.CallbackContext obj)
        {
            isSwiping = false;
        }

    }
}
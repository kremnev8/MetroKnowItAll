using System;
using Model;
using Platformer.Core;
using UnityEngine;
using UnityEngine.EventSystems;
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

        private Vector2 lastPosition;
        private Vector2 moveDelta;

        public float returnSpeed;
        public Vector2 moveAxis;
        public Vector2 closeThreshold;
        public float friction = 0.8f;
        
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
                    Vector2 newPosition = delta * moveAxis.Abs();
                    rectTransform.anchoredPosition = newPosition;
                    moveDelta = newPosition - lastPosition;
                    lastPosition = newPosition;
                }
            }
            else
            {
                UpdateReleased();
            }
        }

        private void UpdateReleased()
        {
            Vector2 _closeThreshold = closeThreshold * rectTransform.sizeDelta;
            
            if ((rectTransform.anchoredPosition * moveAxis).Greater(_closeThreshold) && !isReturning)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (moveDelta.sqrMagnitude < 0.1f || Vector2.Dot(moveDelta.normalized, moveAxis) <= 0)
                {
                    rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, Vector2.zero, returnSpeed * Time.deltaTime);

                    if (rectTransform.anchoredPosition.sqrMagnitude < 1) isReturning = false;
                    moveDelta = Vector2.zero;
                }
                else
                {
                    rectTransform.anchoredPosition += moveDelta;
                    moveDelta *= friction;
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
                Vector2 screenPos = primaryPositionAction.ReadValue<Vector2>();
                if (UIUtil.IsPointerOverUIElement(gameObject, screenPos))
                {
                    initialPos = transform.parent.InverseTransformPoint(screenPos);
                    lastPosition = rectTransform.anchoredPosition;
                    isSwiping = true;
                    isReturning = false;
                }
            }
        }
        
        private void EndSwipe(InputAction.CallbackContext obj)
        {
            isSwiping = false;
        }
    }
}
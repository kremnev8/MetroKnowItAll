using System;
using Gameplay.Questions;
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
        
        public bool isReturning;

        private Vector2 startSwipePos;
        private Vector2 lastDeltaPosition;
        private Vector2 moveDelta;

        private FadeInOut contentFader;
        
        public GameObject content;
        
        [Header("Movement")]
        public float returnSpeed;
        public Vector2 moveAxis;
        public float friction = 0.8f;

        [Header("Positions")] 
        public Vector2 closedPosition;
        public Vector2 closeThreshold;
        public bool shouldDisableContent;
        public bool twoWay;
        
        private void Start()
        {
            GameModel model = Simulation.GetModel<GameModel>();
            input = model.input;

            rectTransform = GetComponent<RectTransform>();
            contentFader = content.GetComponent<FadeInOut>();

            primaryPositionAction = input.actions["primaryPosition"];
            primaryContactAction = input.actions["primaryContact"];
            
            primaryContactAction.started += StartSwipe;
            primaryContactAction.canceled += EndSwipe;
        }

        public void ForceOpen()
        {
            isReturning = true;
            moveDelta = Vector2.zero;
        }

        private void OnEnable()
        {
            if (rectTransform != null)
            {
                isReturning = true;
                rectTransform.anchoredPosition = rectTransform.sizeDelta * moveAxis;
            }

            QuestionController.onQuestionChanged += ForceOpen;
        }

        private void OnDisable()
        {
            QuestionController.onQuestionChanged -= ForceOpen;
        }

        private void Update()
        {
            if (isSwiping)
            {
                Vector2 delta = GetTransformedPosition() - initialPos;
                bool posMove = Vector2.Dot(delta.normalized, moveAxis) > 0;
                if (posMove || twoWay)
                {
                    Vector2 newPosition = startSwipePos + delta * moveAxis.Abs();
                    rectTransform.anchoredPosition = newPosition;
                    moveDelta = newPosition - lastDeltaPosition; 
                    lastDeltaPosition = newPosition;
                }

                Vector2 closedPos = rectTransform.sizeDelta - closedPosition;
                if (posMove && (rectTransform.anchoredPosition * moveAxis).Greater(closedPos))
                {
                    isSwiping = false;
                }
            }
            else
            {
                UpdateReleased();
            }
        }

        private void SetState(bool value)
        {
            if (contentFader == null)
            {
                content.SetActive(value);
            }
            else
            {
                contentFader.Fade(value);
            }
        }

        private void UpdateReleased()
        {
            Vector2 _closeThreshold = rectTransform.sizeDelta - closeThreshold;
            
            if ((rectTransform.anchoredPosition * moveAxis).Greater(_closeThreshold) && !isReturning)
            {
                Vector2 closedPos = rectTransform.sizeDelta * moveAxis + closedPosition;
                rectTransform.anchoredPosition = closedPos;
                if (shouldDisableContent && content.activeSelf)
                {
                    SetState(false);
                }
            }
            else
            {
                if (shouldDisableContent && !content.activeSelf)
                {
                    SetState(true);
                }
                
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
                    lastDeltaPosition = rectTransform.anchoredPosition;
                    startSwipePos = lastDeltaPosition;
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
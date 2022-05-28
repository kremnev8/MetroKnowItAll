using System;
using Gameplay.Conrollers;
using Gameplay.Questions;
using Gameplay.UI;
using Gameplay.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Gameplay.Controls
{
    /// <summary>
    /// UI control that allows to swipe an UI element up and down to open and hide it
    /// </summary>
    public class UISwipe : MonoBehaviour
    {
        private PlayerInput input;
        
        private InputAction primaryPositionAction;
        private InputAction primaryContactAction;

        private RectTransform rectTransform;
        private Vector2 initialPos;
        private bool isSwiping;
        private bool isLocked;
        
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
        
        public Rect bounds;
        public bool clamp;

        private void Start()
        {
            GameModel model = Simulation.GetModel<GameModel>();
            input = model.input;

            rectTransform = GetComponent<RectTransform>();

            if (!content.transform.IsChildOf(transform))
            {
                Debug.LogWarning($"Content {content.name} assigned is not parented to this game object! Disabling!");
                enabled = false;
                return;
            }

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
            isLocked = false;
        }

        public void ForceClosed()
        {
            rectTransform.anchoredPosition = rectTransform.rect.size * moveAxis + closedPosition;
            isLocked = true;
        }

        private void OnEnable()
        {
            if (!content.transform.IsChildOf(transform))
            {
                Debug.LogWarning($"Content {content.name} assigned is not parented to this game object! Disabling!", this);
                enabled = false;
                return;
            }
            
            if (rectTransform != null)
            {
                isReturning = true;
                rectTransform.anchoredPosition = rectTransform.rect.size * moveAxis;
            }

            if (primaryContactAction != null)
            {
                primaryContactAction.started += StartSwipe;
                primaryContactAction.canceled += EndSwipe;
            }
            moveDelta = Vector2.zero;
        }

        private void OnDisable()
        {
            if (primaryContactAction != null)
            {
                primaryContactAction.started -= StartSwipe;
                primaryContactAction.canceled -= EndSwipe;
            }
        }
        

        private void Update()
        {
            if (isSwiping && !isLocked)
            {
                Vector2 delta = GetTransformedPosition() - initialPos;
                bool posMove = Vector2.Dot(delta.normalized, moveAxis) > 0;
                if (posMove || twoWay)
                {
                    Vector2 newPosition = startSwipePos + delta * moveAxis.Abs();
                    if (clamp)
                        newPosition = bounds.Clamp2(newPosition);
                    
                    rectTransform.anchoredPosition = newPosition;
                    moveDelta = newPosition - lastDeltaPosition; 
                    lastDeltaPosition = newPosition;
                }

                Vector2 closedPos = rectTransform.rect.size * moveAxis.Abs() - closedPosition;
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
                contentFader.Fade(value ? FadeDirection.FADE_IN : FadeDirection.FADE_OUT);
            }
        }

        private void UpdateReleased()
        {
            Vector2 _closeThreshold = rectTransform.rect.size * moveAxis.Abs() - closeThreshold;
            
            if ((rectTransform.anchoredPosition * moveAxis).Greater(_closeThreshold) && !isReturning)
            {
                Vector2 closedPos = rectTransform.rect.size * moveAxis + closedPosition;
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
                    rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, Vector2.zero, returnSpeed * Time.unscaledDeltaTime);

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
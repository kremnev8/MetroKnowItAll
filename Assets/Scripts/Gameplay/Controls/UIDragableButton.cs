using Gameplay.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Util;

namespace Gameplay.Controls
{
    /// <summary>
    /// Button that can be dragged, used as part of <see cref="UIDraggableButtonList"/>>
    /// </summary>
    public class UIDragableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public const int gap = 10;
        public float returnSpeed = 5; 

        private bool isDragging;
        private Vector2 initialCursor;
        private float initialY;

        internal UIDraggableButtonList list;
        private PlayerInput input;
        private InputAction primaryPositionAction;
        private RectTransform rectTransform;
        public TMP_Text label;

        public INamedArrayElement element;
        
        public int lastSlot;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            GameModel model = Simulation.GetModel<GameModel>();
            input = model.input;

            primaryPositionAction = input.actions["primaryPosition"];
        }

        public void SetData(INamedArrayElement _element, int index)
        {
            if (_element == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                element = _element;
                label.text = element.displayName;
                label.color = GameController.theme.textColor;
                lastSlot = index;
                
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -index * (rectTransform.sizeDelta.y + gap) - gap);
            }
        }

        public void SetColor(Color color)
        {
            label.color = color;
        }
        
        
        public int GetCurrentSlot()
        {
            return Mathf.RoundToInt((-rectTransform.anchoredPosition.y - gap) / (rectTransform.sizeDelta.y + gap));
        }

        private Vector2 GetTransformedPosition()
        {
            return transform.parent.InverseTransformPoint(primaryPositionAction.ReadValue<Vector2>());
        }

        private void Update()
        {
            if (isDragging)
            {
                Vector2 delta = GetTransformedPosition() - initialCursor;
                
                rectTransform.anchoredPosition = new Vector2(0, list.ClampPos(initialY + delta.y));
            }
            else
            {
                Vector2 newPos = new Vector2(0, -lastSlot * (rectTransform.sizeDelta.y + gap) - gap);
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, newPos, returnSpeed * Time.deltaTime);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            initialCursor = GetTransformedPosition();
            initialY = rectTransform.anchoredPosition.y;
            rectTransform.SetAsLastSibling();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            int currentSlot = GetCurrentSlot();
            if (lastSlot != currentSlot)
            {
                list.Move(lastSlot, currentSlot);
            }
            
        }
    }
}
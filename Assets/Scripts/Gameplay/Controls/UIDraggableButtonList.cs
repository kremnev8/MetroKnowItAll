using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace Gameplay.Controls
{
    /// <summary>
    /// List of buttons that allow to rearrange the data
    /// </summary>
    public class UIDraggableButtonList : MonoBehaviour
    {
        [HideInInspector]
        public List<UIDragableButton> buttons = new List<UIDragableButton>();

        public float minPos;
        public float maxPos;
        
        public UIDragableButton buttonPrefab;
        public UnityEvent onChanged;

        private RectTransform rectTransform;
        
        public void SetData(List<INamedArrayElement> elements)
        {
            if (elements.Count > buttons.Count)
            {
                buttons.Capacity = elements.Count;
                
                for (int i = 0; i < elements.Count; i++)
                {
                    if (i >= buttons.Count)
                    {
                        UIDragableButton button = Instantiate(buttonPrefab, transform);
                        button.list = this;
                        button.lastSlot = i;
                        buttons.Add(button);
                    }
                    buttons[i].SetData(elements[i], i);
                }
            }
            else
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    buttons[i].SetData(elements[i], i);
                }

                for (int i = elements.Count; i < buttons.Count; i++)
                {
                    UIDragableButton button = buttons[i];
                    buttons.RemoveAt(i);
                    i--;
                    Destroy(button.gameObject);
                }
            }

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, elements.Count * 80 + 20);
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        

        public float ClampPos(float pos)
        {
            return Mathf.Clamp(pos, minPos, maxPos);
        }


        public void Move(int oldIndex, int newIndex)
        {
            if (newIndex < 0) newIndex = 0;
            if (newIndex >= buttons.Count) newIndex = buttons.Count - 1;

            if (oldIndex > newIndex)
            {
                for (int i = oldIndex; i > newIndex; i--)
                {
                    Swap(i, i - 1);
                }
            }
            else
            {
                for (int i = oldIndex; i < newIndex; i++)
                {
                    Swap(i, i + 1);
                }
            }
            onChanged?.Invoke();
        }

        private void Swap(int first, int second)
        {
            var firstButton = buttons[first];
            var secondButton = buttons[second];

            buttons[first] = secondButton;
            buttons[second] = firstButton;

            firstButton.lastSlot = second;
            secondButton.lastSlot = first;
        }
    }
}
using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class UIScrollingText : MonoBehaviour
    {
        private TMP_Text text;
        private RectTransform textTrans;
        private RectTransform parent;

        public const float DELAY_BEFORE_SCROLL = 1.5f;
        public const float SCROLL_SPEED = 16;
        
        private bool isEnabled;
        
        private float scrollTime;
        private float scrollDistance;
        private float timeElapsed;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            textTrans = (RectTransform)text.transform;
            parent = (RectTransform)text.transform.parent;
        }

        private void OnEnable()
        {
            timeElapsed = 0;
            
            scrollDistance = text.preferredWidth - parent.rect.width;
            isEnabled = scrollDistance > 0;
            
            if (isEnabled)
            {
                scrollTime = scrollDistance / SCROLL_SPEED;
            }
            else
            {
                textTrans.anchoredPosition = new Vector2(0, 0);
            }
        }

        private void Update()
        {
            if (!isEnabled) return;
            
            timeElapsed += Time.deltaTime;
            if (timeElapsed > DELAY_BEFORE_SCROLL && timeElapsed < DELAY_BEFORE_SCROLL + scrollTime)
            {
                float t = (timeElapsed - DELAY_BEFORE_SCROLL) / scrollTime;
                textTrans.anchoredPosition = new Vector2(-t * scrollDistance, 0);
            }
            else if (timeElapsed > 2 * DELAY_BEFORE_SCROLL + scrollTime && timeElapsed < 2 * DELAY_BEFORE_SCROLL + 2 * scrollTime)
            {
                float t = 1 - (timeElapsed - 2 * DELAY_BEFORE_SCROLL - scrollTime) / scrollTime;
                textTrans.anchoredPosition = new Vector2(-t * scrollDistance, 0);
            }
            else if (timeElapsed > 2 * DELAY_BEFORE_SCROLL + 2 * scrollTime)
            {
                timeElapsed = 0;
            }
        }
    }
}
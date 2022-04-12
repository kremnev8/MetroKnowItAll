using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameplay
{
    public class UIAchievement : MonoBehaviour
    {
        public TMP_Text achievementText;
        public Image achievementIcon;
        public float timeIn;
        public float timeStay;
        public float timeOut;
        
        private RectTransform rectTransform;

        private float timeElapsed;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        public void Popup()
        {
            gameObject.SetActive(true);
            timeElapsed = 0;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private void Update()
        {
            timeElapsed += Time.deltaTime;
            float pos = rectTransform.anchoredPosition.y;
            if (timeElapsed < timeIn)
            {
                float t = timeElapsed / timeIn;
                pos = Mathf.Lerp(0, -rectTransform.sizeDelta.y, t);
            }else if (timeElapsed > timeIn + timeStay && timeElapsed < timeIn + timeStay + timeOut)
            {
                float t = (timeElapsed - timeIn - timeStay) / timeOut;
                pos = Mathf.Lerp(-rectTransform.sizeDelta.y, 0, t);
            }else if (timeElapsed > timeIn + timeStay + timeOut)
            {
                gameObject.SetActive(false);
                return;
            }
            
            rectTransform.anchoredPosition = new Vector2(0, pos);
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(UIAchievement))]
    public class UIAchievementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UIAchievement ui = (UIAchievement) target;
            if (GUILayout.Button("Play"))
            {
                ui.Popup();
            } 
        }
    }
#endif
}
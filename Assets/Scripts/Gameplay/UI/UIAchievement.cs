using System;
using Gameplay.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class UIAchievement : MonoBehaviour
    {
        public static UIAchievement instance;
        
        public TMP_Text achievementName;
        public TMP_Text achievementText;
        public Image achievementIcon;
        public float timeIn;
        public float timeStay;
        public float timeOut;

        public AchievementDB achievements;
        
        private RectTransform rectTransform;

        private float timeElapsed;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            instance = this;
        }

        public static void UnlockAchievement(string key)
        {
            Achievement achievement = instance.achievements.Get(key);
            instance.Popup(achievement);
        }
        
        public void Popup(Achievement achievement)
        {
            achievementName.text = achievement.name;
            achievementText.text = achievement.description;
            achievementIcon.sprite = achievement.icon;
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
}
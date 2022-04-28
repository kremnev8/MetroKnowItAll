using System;
using System.Collections.Generic;
using Gameplay.ScriptableObjects;
using Model;
using Platformer.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gameplay
{
    public class UIAchievement : MonoBehaviour
    {
        public TMP_Text achievementName;
        public TMP_Text achievementText;
        public Image achievementIcon;
        public float timeIn;
        public float timeStay;
        public float timeOut;

        [FormerlySerializedAs("achievements")]
        public AchievementDB achievementData;

        private RectTransform rectTransform;

        private float timeElapsed;
        private Queue<Achievement> achievementQueue = new Queue<Achievement>();

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        public static void UnlockAchievement(string key)
        {
            try
            {
                GameModel model = Simulation.GetModel<GameModel>();
                
                Achievement achievement = model.achievements.achievementData.Get(key);
                if (!model.statistics.current.unlockedAchievements.IsUnlocked(achievement))
                {
                    model.statistics.current.unlockedAchievements.Unlock(achievement);
                    model.achievements.Popup(achievement);
                }
                
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void Popup(Achievement achievement)
        {
            if (gameObject.activeSelf)
            {
                achievementQueue.Enqueue(achievement);
                return;
            }
            
            achievementName.text = achievement.name;
            achievementText.text = achievement.description;
            achievementIcon.sprite = achievement.icon;
            gameObject.SetActive(true);
            timeElapsed = 0;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private void HidePopup()
        {
            gameObject.SetActive(false);
            if (achievementQueue.Count > 0)
            {
                Popup(achievementQueue.Dequeue());
            }
        }

        private void Update()
        {
            timeElapsed += Time.deltaTime;
            float pos = rectTransform.anchoredPosition.y;
            if (timeElapsed < timeIn)
            {
                float t = timeElapsed / timeIn;
                pos = Mathf.Lerp(0, -rectTransform.sizeDelta.y, t);
            }
            else if (timeElapsed > timeIn + timeStay && timeElapsed < timeIn + timeStay + timeOut)
            {
                float t = (timeElapsed - timeIn - timeStay) / timeOut;
                pos = Mathf.Lerp(-rectTransform.sizeDelta.y, 0, t);
            }
            else if (timeElapsed > timeIn + timeStay + timeOut)
            {
                HidePopup();
                return;
            }

            rectTransform.anchoredPosition = new Vector2(0, pos);
        }
    }
}
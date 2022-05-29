using Gameplay.Statistics;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    /// <summary>
    /// UI element responsible for displaying an achievement
    /// </summary>
    public class UIAchievement : MonoBehaviour
    {
        public Sprite lockedIcon;
        
        public TMP_Text title;
        public TMP_Text description;
        public Image iconImage;
        
        public GameObject progressBar;
        public Image barImage;
        public TMP_Text barText;

        public void SetAchievement(Achievement achievement, bool isUnlocked, int progress, int maxProgress)
        {
            title.text = achievement.name;
            description.text = achievement.description;
            iconImage.sprite = isUnlocked ? achievement.icon : lockedIcon;
            iconImage.color = isUnlocked ? achievement.spriteTint : Color.white;

            bool showBar = achievement.hasProgress && !isUnlocked;
            
            progressBar.SetActive(showBar);
            if (showBar)
            {
                barImage.fillAmount = progress / (float)maxProgress;
                barText.text = $"{progress} / {maxProgress}";
            }
        }
        
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    /// <summary>
    /// UI element displaying a statistic
    /// </summary>
    public class UIStatisticView : MonoBehaviour
    {
        public TMP_Text label;
        public TMP_Text valueText;

        public GameObject progressBar;
        public Image fillImage;

        public void DisplayProgress(string name, int count, int max)
        {
            float fill = count / (float)max;
            int percent = Mathf.RoundToInt(fill * 100);
            
            label.text = name;
            valueText.text = $"{count}  ( {percent}% )";
            fillImage.fillAmount = fill;
            progressBar.SetActive(true);
        }
        
        public void DisplayStatistic(string name, float value, string unit = "")
        {
            label.text = name;
            valueText.text = $"{value} {unit}";
            progressBar.SetActive(false);
        }
        
        public void DisplayStatistic(string name, int value, string unit = "")
        {
            label.text = name;
            valueText.text = $"{value} {unit}";
            progressBar.SetActive(false);
        }

    }
}
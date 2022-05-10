using System;
using System.Collections.Generic;
using System.Text;
using Gameplay.MetroDisplay.Model;
using Gameplay.Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay.UI
{
    /// <summary>
    /// Struct for data about one score point
    /// </summary>
    public struct ScoreItem
    {
        public string name;
        public string stateText;
        public int points;

        public ScoreItem(string name, string stateText, int points)
        {
            this.name = name;
            this.stateText = stateText;
            this.points = points;
        }
    }
    
    /// <summary>
    /// Controls how game over screen is displayed.
    /// </summary>
    public class UIGameOverScreen : MonoBehaviour
    {
        public Transform statsTrans;
        public TMP_Text totalScoreText;
        public TMP_Text unlockedStationsText;

        public UIScoreItem scoreItemPrefab;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Popup(StatisticsEntry statistics, List<MetroStation> unlockedStations)
        {
            statsTrans.gameObject.ClearChildren();
            
            List<ScoreItem> score = statistics.CalculateScore();
            int totalScore = 0;
            foreach (ScoreItem item in score)
            {
                UIScoreItem scoreDisplay = Instantiate(scoreItemPrefab, statsTrans);
                scoreDisplay.scoreLabel.text = $"{item.name} - {item.stateText}";
                scoreDisplay.scoreCountText.text = $"{item.points:000} оч.";
                totalScore += item.points;
            }

            totalScoreText.text = $"Всего {totalScore} оч.";

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < unlockedStations.Count; i++)
            {
                MetroStation station = unlockedStations[i];
                builder.Append(station.currentName);
                if (i != unlockedStations.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            unlockedStationsText.text = builder.ToString();
            gameObject.SetActive(true);
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

    }
}
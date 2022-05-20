﻿using System;
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
        public GameObject arcadeScreen;
        public GameObject learningScreen;

        public Transform statsTrans;
        public TMP_Text totalScoreText;
        public UIScoreItem scoreItemPrefab;

        public TMP_Text correctText;
        public TMP_Text ticketsText;
        public new UIFillAnimation animation;

        private void Awake()
        {
            arcadeScreen.SetActive(false);
            learningScreen.SetActive(false);
        }

        public void PopupArcade(StatisticsEntry statistics, List<MetroStation> unlockedStations)
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
            arcadeScreen.SetActive(true);
            learningScreen.SetActive(false);
        }
        
        public void PopupLearning(int correct, int total, int tickets)
        {
            statsTrans.gameObject.ClearChildren();

            correctText.text = $"Вы ответили верно\nна {correct} из {total} вопросов!";
            ticketsText.text = $"Получено {tickets} билетов";
            animation.Display(0, correct / (float)total);
            
            arcadeScreen.SetActive(false);
            learningScreen.SetActive(true);
        }

    }
}
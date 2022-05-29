using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using Gameplay.Statistics;
using Gameplay.UI;
using ScriptableObjects;
using Newtonsoft.Json;
using UnityEngine;
using Util;

namespace Gameplay.Conrollers
{
    /// <summary>
    /// Manages statistics and achievements. Also saves and loads statistics data to disk
    /// </summary>
    public class StatisticsController : SaveDataBaseController<StatisticsEntry>
    {
        public StatisticsEntry sesion;
        public AchievementDB achievements;

        public override int Version => 2;
        public override string Filename => "statistics";
        public override void OnVersionChanged(int oldVersion)
        {
            if (oldVersion <= 0)
            {
                current.correctAnswers = 0;
            }

            if (oldVersion <= 1)
            {
                Debug.Log("migrating version");
                current.unlockedStations = new BoolRecord<MetroStation, int>();
            }
        }

        private void Start()
        {
            EventManager.StartListening(EventTypes.SESSION_STARTED, OnNewGame);
            EventManager.StartListening(EventTypes.QUESTION_ANSWERED, OnAnswer);
        }

        private void OnDestroy()
        {
            EventManager.StopListening(EventTypes.SESSION_STARTED, OnNewGame);
            EventManager.StopListening(EventTypes.QUESTION_ANSWERED, OnAnswer);
        }

        public override void InitializeSaveData(StatisticsEntry data)
        {
            
        }

        public override void OnSaveDataLoaded()
        {
        }

        public void TryUnlockStation(MetroStation station)
        {
            if (!current.unlockedStations.IsUnlocked(station))
            {
                current.unlockedStations.Unlock(station);
            }
        }

        public void OnNewGame(object[] objects)
        {
            if (sesion != null)
            {
                current.Append(sesion);
                
                int score = sesion.CalculateScore().Select(item => item.points).Sum();
                current.maxScore = Mathf.Max(current.maxScore, score);
                if (current.maxScore >= 1000)
                {
                    UIAchievementPopup.UnlockAchievement("MorePoints");
                }
            }

            sesion = new StatisticsEntry();
        }

        private void OnAnswer(object[] objects)
        {
            bool result = (bool)objects[0];
            float timeElapsed = (float)objects[1];
            
            if (result)
            {
                OnCorrectAnswer(timeElapsed);
            }
            else
            {
                OnWrongAnswer(timeElapsed);
            }

            foreach (ScriptableAchievementProvider provider in achievements.providers)
            {
                provider.CheckAchievementsState(achievements, current);
            }
        }

        private void OnCorrectAnswer(float time)
        {
            sesion.correctAnswers++;
            sesion.totalAnswers++;
            
            sesion.correctAnswerStreak++;
            sesion.logestCorrectAnswerStreak = Mathf.Max(sesion.logestCorrectAnswerStreak, sesion.correctAnswerStreak);
            
            sesion.fastestCorrectAnswer = sesion.fastestCorrectAnswer > 0 ? Mathf.Min(sesion.fastestCorrectAnswer, time) : time;
            sesion.maximumCorrectAnswerTime = Mathf.Max(sesion.maximumCorrectAnswerTime, time);
            sesion.averageAnswerTime = sesion.averageAnswerTime.Average(time, sesion.totalAnswers);

            if (sesion.correctAnswerStreak == 5)
            {
                UIAchievementPopup.UnlockAchievement("PerfectFive");
            }
            if (sesion.fastestCorrectAnswer < 7)
            {
                UIAchievementPopup.UnlockAchievement("QuickAnswer");
            }

            if (sesion.maximumCorrectAnswerTime > 80)
            {
                UIAchievementPopup.UnlockAchievement("SlowThinking");
            }
            

            switch (sesion.correctAnswers)
            {
                case 10:
                    UIAchievementPopup.UnlockAchievement("Correct10");
                    break;
                case 50:
                    UIAchievementPopup.UnlockAchievement("Correct50");
                    break;
                case 100:
                    UIAchievementPopup.UnlockAchievement("Correct100");
                    break;
                case 200:
                    UIAchievementPopup.UnlockAchievement("Correct200");
                    break;
            }
        }

        private void OnWrongAnswer(float time)
        {
            sesion.totalAnswers++;
            sesion.correctAnswerStreak = 0;
            
            sesion.averageAnswerTime = sesion.averageAnswerTime.Average(time, sesion.totalAnswers);
            
            UIAchievementPopup.UnlockAchievement("MakeAMistake");
        }
        
    }
}
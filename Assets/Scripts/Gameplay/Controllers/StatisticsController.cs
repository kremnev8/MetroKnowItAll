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
        [SerializeField] private Metro metro;
        [SerializeField] private AchievementDB achievements;

        public StatisticsEntry sesion;
        public List<MetroStation> sesionUnlockedStations = new List<MetroStation>();

        public override int Version => 1;
        public override string Filename => "statistics";
        public override void OnVersionChanged(int oldVersion)
        {
            if (oldVersion <= 0)
            {
                current.correctAnswers = 0;
            }

            if (oldVersion <= 1)
            {
                current.unlockedStations = new BoolRecord<MetroStation, int>();
                current.unlockedLines = new BoolRecord<MetroLine, int>();
            }
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventTypes.SESSION_STARTED, OnNewGame);
            EventManager.StartListening(EventTypes.QUESTION_ANSWERED, OnAnswer);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventTypes.SESSION_STARTED, OnNewGame);
            EventManager.StopListening(EventTypes.QUESTION_ANSWERED, OnAnswer);
        }

        public override void InitializeSaveData(StatisticsEntry data)
        {
                
            List<MetroStation> stations = new List<MetroStation>(metro.lines.Count * 30);
            foreach (MetroLine line in metro.lines)
            {
                stations.AddRange(line.stations);
            }
        }

        public override void OnSaveDataLoaded()
        {
        }

        public void TryUnlockStation(MetroStation station)
        {
            if (!current.unlockedStations.IsUnlocked(station) && 
                !sesionUnlockedStations.Contains(station))
            {
                sesionUnlockedStations.Add(station);
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
                    UIAchievement.UnlockAchievement("MorePoints");
                }
                foreach (MetroStation station in sesionUnlockedStations)
                {
                    current.unlockedStations.Unlock(station);
                }

                IEnumerable<byte> lines = sesionUnlockedStations.Select(station => station.lineId).Distinct();
                foreach (byte lineId in lines)
                {
                    MetroLine line = metro.lines[lineId];
                    if (line.stations.All(station => current.unlockedStations.IsUnlocked(station)))
                    {
                        current.unlockedLines.Unlock(line);
                    }
                }
            }

            sesion = new StatisticsEntry();
            sesionUnlockedStations.Clear();
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
                UIAchievement.UnlockAchievement("PerfectFive");
            }
            if (sesion.fastestCorrectAnswer < 7)
            {
                UIAchievement.UnlockAchievement("QuickAnswer");
            }

            if (sesion.maximumCorrectAnswerTime > 80)
            {
                UIAchievement.UnlockAchievement("SlowThinking");
            }
            

            switch (sesion.correctAnswers)
            {
                case 10:
                    UIAchievement.UnlockAchievement("Correct10");
                    break;
                case 50:
                    UIAchievement.UnlockAchievement("Correct50");
                    break;
                case 100:
                    UIAchievement.UnlockAchievement("Correct100");
                    break;
                case 200:
                    UIAchievement.UnlockAchievement("Correct200");
                    break;
            }
        }

        private void OnWrongAnswer(float time)
        {
            sesion.totalAnswers++;
            sesion.correctAnswerStreak = 0;
            
            sesion.averageAnswerTime = sesion.averageAnswerTime.Average(time, sesion.totalAnswers);
            
            UIAchievement.UnlockAchievement("MakeAMistake");
        }
        
    }
}
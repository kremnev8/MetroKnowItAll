using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gameplay.ScriptableObjects;
using Newtonsoft.Json;
using UnityEngine;

namespace Gameplay.Statistics
{
    public class StatisticsManager : MonoBehaviour
    {
        public Statistics current;

        [SerializeField] private Metro metro;
        [SerializeField] private AchievementDB achievements;
        
        private void Awake()
        {
            string dataPath = Application.persistentDataPath + "/statistics.json";
            if (File.Exists(dataPath))
            {
                string json = File.ReadAllText(dataPath, Encoding.UTF8);
                current = JsonConvert.DeserializeObject<Statistics>(json);
            }
            else
            {
                current = new Statistics();
                current.unlockedLines.Prepare(metro.lines);
                
                List<MetroStation> stations = new List<MetroStation>(metro.lines.Count * 30);
                foreach (MetroLine line in metro.lines)
                {
                    stations.AddRange(line.stations);
                }

                current.unlockedStations.Prepare(stations);
                current.unlockedAchievements.Prepare(achievements.GetAll());
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                string dataPath = Application.persistentDataPath + "/statistics.json";
                string json = JsonConvert.SerializeObject(current);
                File.WriteAllText(dataPath, json, Encoding.UTF8);
            }
        }

        public void OnCorrectAnswer(float time)
        {
            current.correctAnswers++;
            current.fastestCorrectAnswer = Mathf.Min(current.fastestCorrectAnswer, time);
            if (current.fastestCorrectAnswer < 7)
            {
                UIAchievement.UnlockAchievement("QuickAnswer");
            }

            switch (current.correctAnswers)
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

        public void OnWrongAnswer()
        {
            UIAchievement.UnlockAchievement("MakeAMistake");
        }
        
    }
}
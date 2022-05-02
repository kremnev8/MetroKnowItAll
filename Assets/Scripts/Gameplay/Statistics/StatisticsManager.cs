using System.Collections.Generic;
using System.IO;
using System.Text;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using Gameplay.UI;
using ScriptableObjects;
using Newtonsoft.Json;
using UnityEngine;
using Util;

namespace Gameplay.Statistics
{
    /// <summary>
    /// Manages statistics and achievements. Also saves and loads statistics data to disk
    /// </summary>
    public class StatisticsManager : MonoBehaviour
    {
        public const int version = 1;
        
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
                if (current.dataVersion == 0)
                {
                    current.correctAnswers = 0;
                }
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
            current.dataVersion = version;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void Save()
        {
            if (current != null)
            {
                string dataPath = Application.persistentDataPath + "/statistics.json";
                string json = JsonConvert.SerializeObject(current);
                File.WriteAllText(dataPath, json, Encoding.UTF8);
            }
        }

        public void OnCorrectAnswer(float time)
        {
            current.correctAnswers++;
            current.totalAnswers++;
            
            current.correctAnswerStreak++;
            
            current.fastestCorrectAnswer = current.fastestCorrectAnswer > 0 ? Mathf.Min(current.fastestCorrectAnswer, time) : time;
            current.maximumCorrectAnswerTime = Mathf.Max(current.fastestCorrectAnswer, time);
            current.averageAnswerTime = current.averageAnswerTime.Average(time, current.totalAnswers);

            if (current.correctAnswerStreak == 5)
            {
                UIAchievement.UnlockAchievement("PerfectFive");
            }
            if (current.fastestCorrectAnswer < 7)
            {
                UIAchievement.UnlockAchievement("QuickAnswer");
            }

            if (current.maximumCorrectAnswerTime > 80)
            {
                UIAchievement.UnlockAchievement("SlowThinking");
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

        public void OnWrongAnswer(float time)
        {
            current.totalAnswers++;
            current.correctAnswerStreak = 0;
            
            current.averageAnswerTime = current.averageAnswerTime.Average(time, current.totalAnswers);
            
            UIAchievement.UnlockAchievement("MakeAMistake");
        }
        
    }
}
using System.Collections.Generic;
using System.Linq;
using Gameplay.Conrollers;
using Gameplay.MetroDisplay.Model;
using Gameplay.UI;
using Newtonsoft.Json;
using ScriptableObjects;
using UnityEngine;
using Util;

namespace Gameplay.Statistics
{
    /// <summary>
    /// Model class for game statistics data
    /// </summary>
    public class StatisticsEntry : ISaveData
    {
        public int Version { get; set; }
        public BoolRecord<MetroStation, int> unlockedStations = new BoolRecord<MetroStation, int>();
        public BoolRecord<MetroLine, int> unlockedLines = new BoolRecord<MetroLine, int>();

        public BoolRecord<Achievement, string> unlockedAchievements = new BoolRecord<Achievement, string>();

        public int correctAnswers;
        public int totalAnswers;
        
        public int correctAnswerStreak;
        public int logestCorrectAnswerStreak;
        
        public float fastestCorrectAnswer;
        public float averageAnswerTime;
        public float maximumCorrectAnswerTime;

        public int maxScore;
        public int tickets;

        public List<ScoreItem> CalculateScore()
        {
            float correctMultiplier = correctAnswers / (float)totalAnswers;
            List<ScoreItem> scoreItems = new List<ScoreItem>
            {
                new ScoreItem( "Всего верных ответов", correctAnswers.ToString(),correctAnswers * 50),
                new ScoreItem( "Среднее время ответа", $"{averageAnswerTime:0.00}с",Mathf.Max(0, Mathf.RoundToInt((200 + (10 - averageAnswerTime) * 50)))),
                new ScoreItem( "Процент верных ответов", $"{correctAnswers}/{totalAnswers}",Mathf.RoundToInt(correctMultiplier * 1000))
            };

            return scoreItems;
        }

        public void Append(StatisticsEntry other)
        {
            averageAnswerTime = averageAnswerTime.Average(other.averageAnswerTime, totalAnswers, other.totalAnswers);
            
            correctAnswers += other.correctAnswers;
            totalAnswers += other.totalAnswers;

            correctAnswerStreak = other.correctAnswerStreak;
            logestCorrectAnswerStreak = Mathf.Max(logestCorrectAnswerStreak, other.logestCorrectAnswerStreak);
            
            fastestCorrectAnswer = fastestCorrectAnswer > 0 ? Mathf.Min(fastestCorrectAnswer, other.fastestCorrectAnswer) : other.fastestCorrectAnswer;
            maximumCorrectAnswerTime = Mathf.Max(maximumCorrectAnswerTime, other.maximumCorrectAnswerTime);
        }
    }
}
using System;
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
    [AttributeUsage(AttributeTargets.Field)]
    public class StatisticsMetadataAttribute : Attribute
    {
        public string name;
        public string unit;
        public string maxField;

        public StatisticsMetadataAttribute(string name)
        {
            this.name = name;
            maxField = "";
            unit = "";
        }
        
        public StatisticsMetadataAttribute(string name, string unit)
        {
            this.unit = unit;
            this.name = name;
            maxField = "";
        }

        public StatisticsMetadataAttribute(string name, string unit, string maxField)
        {
            this.unit = unit;
            this.name = name;
            this.maxField = maxField;
        }
    }
    
    /// <summary>
    /// Model class for game statistics data
    /// </summary>
    public class StatisticsEntry : ISaveData
    {
        public int Version
        {
            get => version;
            set => version = value;
        }

        public BoolRecord<MetroStation, int> unlockedStations = new BoolRecord<MetroStation, int>();
        public BoolRecord<Achievement, string> unlockedAchievements = new BoolRecord<Achievement, string>();

        [StatisticsMetadata("Верных ответов","", nameof(totalAnswers))]
        public int correctAnswers;
        public int totalAnswers;
        
        [StatisticsMetadata("Верных ответов подряд")]
        public int correctAnswerStreak;
        public int logestCorrectAnswerStreak;
        
        [StatisticsMetadata("Самый быстрый ответ", "c")]
        public float fastestCorrectAnswer;
        [StatisticsMetadata("Среднее время ответа", "c")]
        public float averageAnswerTime;
        [StatisticsMetadata("Самый долгий ответ", "c")]
        public float maximumCorrectAnswerTime;

        [StatisticsMetadata("Лучший счет", "оч.")]
        public int maxScore;
        [StatisticsMetadata("Достигнутая эра", "г.")]
        public int lastReachedYear;

        public int version;

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
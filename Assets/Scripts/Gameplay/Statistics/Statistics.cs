using System;
using System.Collections.Generic;
using Gameplay.ScriptableObjects;

namespace Gameplay.Statistics
{
    public class Statistics
    {
        public BoolRecord<MetroStation, int> unlockedStations = new BoolRecord<MetroStation, int>();
        public BoolRecord<MetroLine, int> unlockedLines = new BoolRecord<MetroLine, int>();

        public BoolRecord<Achievement, string> unlockedAchievements = new BoolRecord<Achievement, string>();

        public int correctAnswers;
        public float fastestCorrectAnswer;
    }
}
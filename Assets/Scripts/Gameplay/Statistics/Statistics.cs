using Gameplay.MetroDisplay.Model;
using ScriptableObjects;

namespace Gameplay.Statistics
{
    public class Statistics
    {
        public int dataVersion;
        public BoolRecord<MetroStation, int> unlockedStations = new BoolRecord<MetroStation, int>();
        public BoolRecord<MetroLine, int> unlockedLines = new BoolRecord<MetroLine, int>();

        public BoolRecord<Achievement, string> unlockedAchievements = new BoolRecord<Achievement, string>();

        public int correctAnswers;
        public int totalAnswers;
        
        public int correctAnswerStreak;
        
        public float fastestCorrectAnswer;
        public float averageAnswerTime;
    }
}
using System;
using System.Collections.Generic;
using Gameplay.Statistics;
using UnityEngine;

namespace ScriptableObjects
{
    public class WrongProviderException : Exception{

        public WrongProviderException() : base("This provider can't return progress")
        {
            
        }
    }
    
    public abstract class ScriptableAchievementProvider : ScriptableObject
    {
        public string baseId;
        public Sprite baseIcon;
        
        public abstract List<Achievement> GetAchievements();
        public abstract Progress GetProgress(Achievement achievement, StatisticsEntry entry);
        public abstract void CheckAchievementsState(AchievementDB achievements, StatisticsEntry entry); 
    }

    public struct Progress
    {
        public int current;
        public int max;

        public Progress(int current, int max)
        {
            this.current = current;
            this.max = max;
        }
    }
}
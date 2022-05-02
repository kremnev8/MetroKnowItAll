using System;
using Gameplay.Statistics;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Defines an achievement
    /// </summary>
    [Serializable]
    public class Achievement : GenericItem, IIndexable<string>
    {
        public string name;
        public string id;
        public string description;
        public Sprite icon;
        
        public string ItemId => id;
        public string index => id;
    }
    
    /// <summary>
    /// Data store for existing achievements
    /// </summary>
    [CreateAssetMenu(fileName = "Achievement DB", menuName = "SO/New Achievement DB", order = 0)]
    public class AchievementDB : GenericDB<Achievement>
    {
        
    }
}
using System;
using Gameplay.Statistics;
using UnityEngine;

namespace ScriptableObjects
{
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
    
    [CreateAssetMenu(fileName = "Achievement DB", menuName = "SO/New Achievement DB", order = 0)]
    public class AchievementDB : GenericDB<Achievement>
    {
        
    }
}
using System;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay.ScriptableObjects
{
    [Serializable]
    public class Achievement : GenericItem
    {
        public string name;
        public string id;
        public string description;
        public Sprite icon;
        
        public string ItemId => id;
    }
    
    [CreateAssetMenu(fileName = "Achievement DB", menuName = "SO/New Achievement DB", order = 0)]
    public class AchievementDB : GenericDB<Achievement>
    {
        
    }
}
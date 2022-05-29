using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Difficulty model class
    /// </summary>
    [Serializable]
    public class Difficulty
    {
        public string difficultyName;
        public int maxAttempts;
        public int partialPerAttempt;
        public bool allowHints;
    }
    
    /// <summary>
    /// Data store for difficulties
    /// </summary>
    [CreateAssetMenu(fileName = "Difficulty", menuName = "SO/New Difficulty", order = 0)]
    public class DifficultyConfig : ScriptableObject
    {
        public List<Difficulty> difficulties = new List<Difficulty>();
    }
}
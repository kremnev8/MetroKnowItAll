using System;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Defines a simple question
    /// </summary>
    [Serializable]
    public class Question : GenericItem
    {
        public string itemId;
        public string ItemId => itemId;

        public string QuestionText;
        public string QuestionAnswer;
    }
    
    /// <summary>
    /// Data store for simple questions
    /// </summary>
    [CreateAssetMenu(fileName = "Question DB", menuName = "SO/New Question DB", order = 0)]
    public class QuestionsDB : GenericDB<Question>
    {
        
    }
}
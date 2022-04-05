using Gameplay;
using Gameplay.Questions;
using UnityEngine.InputSystem;


namespace Model
{
    [System.Serializable]
    public class GameModel
    {
        public PlayerInput input;
        public MetroRenderer renderer;
        public QuestionController questions;

    }
}
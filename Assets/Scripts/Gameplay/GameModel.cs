using Gameplay;
using Gameplay.Questions;
using Gameplay.Statistics;
using UnityEngine.InputSystem;


namespace Model
{
    [System.Serializable]
    public class GameModel
    {
        public PlayerInput input;
        public TouchCameraController cameraController;
        public MetroRenderer renderer;
        public QuestionController questions;
        public ColorPalette palette;
        public StatisticsManager statistics;
        public UIAchievement achievements;

    }
}
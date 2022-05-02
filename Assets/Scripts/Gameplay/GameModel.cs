using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.Questions;
using Gameplay.Statistics;
using Gameplay.UI;
using ScriptableObjects;
using UnityEngine.InputSystem;


namespace Gameplay
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
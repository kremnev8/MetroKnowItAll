using Gameplay.Conrollers;
using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.Questions;
using Gameplay.Statistics;
using Gameplay.UI;
using ScriptableObjects;
using UnityEngine.InputSystem;


namespace Gameplay
{
    /// <summary>
    /// Game model contains all important game classes and stores them for easy access from anywhere
    /// </summary>
    [System.Serializable]
    public class GameModel
    {
        public PlayerInput input;
        public TouchCameraController cameraController;
        
        public MetroRenderer renderer;
        public QuestionController questions;
        public ColorPalette palette;
        
        public StatisticsManager statistics;
        public SettingsController settings;
        
        public UIAchievement achievements;
        public UIGameOverScreen gameOverScreen;

    }
}
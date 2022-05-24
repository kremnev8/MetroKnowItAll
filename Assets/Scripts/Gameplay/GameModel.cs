using Gameplay.Conrollers;
using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.Questions;
using Gameplay.Statistics;
using Gameplay.UI;
using ScriptableObjects;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


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

        public UIGame uiGame;
        
        public MetroRenderer renderer;
        public GameModeController gameModeController;
        public ColorPalette palette;
        
        public StatisticsController statistics;
        public SettingsController settings;
        
        [FormerlySerializedAs("achievements")] public UIAchievementPopup achievementsPopup;
        public UIGameOverScreen gameOverScreen;

    }
}
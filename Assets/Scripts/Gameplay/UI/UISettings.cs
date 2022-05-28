using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Conrollers;
using Gameplay.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay.UI
{
    public class UISettings : MonoBehaviour
    {
        public Toggle themeToggle;
        public EventToggleGroup difficultyToggleGroup;
        public GameObject crossfade;

        public GameObject exitDialogWindow;
        public GameObject warningIcon;
        public TMP_Text exitDialogText;

        private GameModel model;
        private GameModeController gameModeController;

        private void Start()
        {
            model = Simulation.GetModel<GameModel>();

            themeToggle.SetIsOnWithoutNotify(model.settings.current.theme == 0);
            difficultyToggleGroup.SetToggle(model.settings.current.difficulty);
            gameModeController = model.gameModeController;
        }

        public void SetDifficulty(int level)
        {
            model.settings.current.difficulty = level;
            model.settings.MarkDirty();
        }

        public void SetTheme(bool light)
        {
            model.settings.current.theme = light ? 0 : 1;
            model.settings.MarkDirty();
        }
        
        public void LoadMenu()
        {
            if (SceneTransitionManager.instance != null)
            {
                SceneTransitionManager.instance.LoadMenu();
            }
        }

        public void TryExitGame()
        {
            if (gameModeController.gameState.isPlaying)
            {
                exitDialogText.text = "Весь несохраненный прогресс будет потерян!";
                exitDialogText.color = Color.red;
                warningIcon.SetActive(true);
            }
            else
            {
                exitDialogText.text = "Выйти в главное меню?";
                exitDialogText.color = GameController.theme.textColor;
                warningIcon.SetActive(false);
            }

            exitDialogWindow.SetActive(true);
        }
    }
}
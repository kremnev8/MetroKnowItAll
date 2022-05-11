using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay.UI
{
    public class UISettings : MonoBehaviour
    {
        public Toggle themeToggle;
        public EventToggleGroup difficultyToggleGroup;
        
        private GameModel model;

        private void Start()
        {
            model = Simulation.GetModel<GameModel>();
            
            themeToggle.SetIsOnWithoutNotify(model.settings.current.theme == 0);
            difficultyToggleGroup.SetToggle(model.settings.current.difficulty);
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

        public void LoadGame()
        {
            if (SceneTransitionManager.instance != null)
            {
                SceneTransitionManager.instance.StartGame();
            }
        }
        
        public void LoadMenu()
        {
            if (SceneTransitionManager.instance != null)
            {
                SceneTransitionManager.instance.LoadMenu();
            }
        }
    }
}
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
        
        public GameObject crossfade;
        
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

        public void LoadGame(int gameMode)
        {
            Debug.Log(SceneTransitionManager.instance != null);
            if (SceneTransitionManager.instance != null)
            {
                if (crossfade != null)
                    crossfade.SetActive(true);
                
                SceneTransitionManager.instance.StartGame(gameMode);
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
using System;
using System.Collections;
using Gameplay.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager instance;

        private static int gameMode;
        

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
        
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartGame(int newGameMode)
        {
            gameMode = newGameMode;
            StartCoroutine(LoadLevel("Game"));
        }

        public void LoadMenu()
        {
            Time.timeScale = 1f;
            StartCoroutine(LoadLevel("MainMenu"));
        }

        IEnumerator LoadLevel(string levelName)
        {
            var async = SceneManager.LoadSceneAsync(levelName);
            async.allowSceneActivation = false;

            while (Math.Abs(async.progress - 0.9f) > 0.01f)
            {
                yield return null;
            }
            
            async.allowSceneActivation = true;
            while (Simulation.GetModel<GameModel>().gameModeController == null)
            {
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Starting!");
            Simulation.GetModel<GameModel>().gameModeController.StartGame(gameMode);

        }
    }
}
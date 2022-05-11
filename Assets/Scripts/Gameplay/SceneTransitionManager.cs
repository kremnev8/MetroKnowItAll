using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager instance;


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

        public void StartGame()
        {
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

            yield return new WaitForSeconds(1);
            async.allowSceneActivation = true;
            
        }
    }
}
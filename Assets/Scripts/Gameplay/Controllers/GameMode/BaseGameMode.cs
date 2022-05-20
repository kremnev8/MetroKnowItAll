using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Gameplay.Conrollers
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class BaseGameMode : MonoBehaviour
    {
        public virtual int LatestVersion => 1;
        
        [JsonProperty] 
        [HideInInspector]
        public int Version;
        
        [JsonProperty]
        [HideInInspector]
        public bool isInitialized;
        
        
        public void StartSession(Game gameState)
        {
            bool hasSaveData = Load();
            if (!hasSaveData)
            {
                SetupNewSession(gameState);
            }

            SceneTransitionManager.sceneUnloaded += OnSceneUnloaded;
            isInitialized = true;
            ContinueSession(gameState);
        }

        private void OnSceneUnloaded()
        {
            Save(); 
        }


        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }
        
        private void OnDestroy()
        {
            SceneTransitionManager.sceneUnloaded -= OnSceneUnloaded;
        }

        protected bool Load()
        {
            string dataPath = $"{Application.persistentDataPath}/saves/{gameModeId}-save.json";
            string dirPath = Path.GetDirectoryName(dataPath);
            Directory.CreateDirectory(dirPath);
            if (File.Exists(dataPath))
            {
                try
                {
                    string json = File.ReadAllText(dataPath, Encoding.UTF8);
                    JsonConvert.PopulateObject(json, this);
                    if (Version < LatestVersion)
                    {
                        OnVersionChanged(Version);
                    }
                    Version = LatestVersion;
                    return true;
                }
                catch (Exception e)
                {
                    Debug.Log($"Failed to deserialize {gameModeId} mode save file. Creating new one!");
                    Debug.Log(e);
                }
            }

            Version = LatestVersion;
            return false;
        }

        protected void Save()
        {
            if (isInitialized)
            {
                Debug.Log("Saving!!");
                string dataPath = $"{Application.persistentDataPath}/saves/{gameModeId}-save.json";
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(dataPath, json, Encoding.UTF8);
            }
        }
        
        public abstract void Init(GameModeController mainController);
        public abstract string gameModeId { get; }
        public abstract void SetupNewSession(Game gameState);
        public abstract void ContinueSession(Game gameState);
        public abstract void ConfirmPressed();
        public abstract void StartGamePressed();
        public abstract void ManualUpdate();
        public abstract string GetNextTip(int index);
        
        public abstract void OnVersionChanged(int oldVersion);
    }
}
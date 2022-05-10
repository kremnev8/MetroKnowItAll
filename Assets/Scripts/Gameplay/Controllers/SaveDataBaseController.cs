using System.Collections.Generic;
using System.IO;
using System.Text;
using Gameplay.MetroDisplay.Model;
using Gameplay.Statistics;
using Newtonsoft.Json;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay.Conrollers
{
    public interface ISaveData
    {
        [JsonIgnore]
        public int Version { get; set; }
    }
    
    public abstract class SaveDataBaseController<T> : MonoBehaviour
    where T : class, ISaveData, new()
    {
        public T current;

        private void Awake()
        {
            string dataPath = $"{Application.persistentDataPath}/{Filename}.json";
            if (File.Exists(dataPath))
            {
                string json = File.ReadAllText(dataPath, Encoding.UTF8);
                current = JsonConvert.DeserializeObject<T>(json);
                if (current.Version < Version)
                {
                    OnVersionChanged(current.Version);
                }
            }
            else
            {
                current = new T();
                InitializeSaveData(current);
            }
            current.Version = Version;
        }

        public abstract int Version { get; }
        public abstract string Filename { get; }

        public abstract void OnVersionChanged(int oldVersion);
        public abstract void InitializeSaveData(T data);
        public abstract void OnSaveDataLoaded();

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

        private void Save()
        {
            if (current != null)
            {
                string dataPath = $"{Application.persistentDataPath}/{Filename}.json";
                string json = JsonConvert.SerializeObject(current, Formatting.Indented);
                File.WriteAllText(dataPath, json, Encoding.UTF8);
            }
        }
    }
}
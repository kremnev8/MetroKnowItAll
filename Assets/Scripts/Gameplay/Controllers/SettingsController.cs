﻿using System;
using Newtonsoft.Json;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay.Conrollers
{
    /// <summary>
    /// Settings data model
    /// </summary>
    [Serializable]
    public class SettingsEntry : ISaveData
    {
        public int dataVersion;
        
        [JsonIgnore]
        public int Version
        {
            get => dataVersion;
            set => dataVersion = value;
        }

        public int difficulty;
        public int theme;
    }
    
    /// <summary>
    /// Controller that handles saving and loading player settings
    /// </summary>
    public class SettingsController : SaveDataBaseController<SettingsEntry>
    {
        public DifficultyConfig difficultyConfig;
        public ColorPalette palette;

        public Difficulty currentDifficulty => difficultyConfig.difficulties[current.difficulty];

        public static Action<Difficulty> difficultyChanged;

        public override int Version => 1;
        public override string Filename => "settings";
        public override void OnVersionChanged(int oldVersion)
        {
        }

        public override void InitializeSaveData(SettingsEntry data)
        {
        }

        public override void OnSaveDataLoaded()
        {
            palette.themeIndex = current.theme;
        }

        private void Start()
        {
            difficultyChanged?.Invoke(currentDifficulty);
        }

        public void MarkDirty()
        {
            palette.themeIndex = current.theme;
            difficultyChanged?.Invoke(currentDifficulty);
            Save();
        }
    }
}
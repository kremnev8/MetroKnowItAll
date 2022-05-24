using System;
using System.Reflection;
using Gameplay.Conrollers;
using Gameplay.Core;
using Gameplay.Statistics;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay.UI
{
    public class UIStatisticsWindow : MonoBehaviour
    {
        public AchievementDB achievements;
        
        public GameObject statisticsRoot;
        public GameObject achievementsRoot;

        public UIStatisticView statisticPrefab;
        public Transform statisticsContent;

        public UIAchievement achievementPrefab;
        public Transform achievementContent;

        private GameModel model;
        private StatisticsController statistics;

        public Toggle achievementToggle;

        private void Start()
        {
            model = Simulation.GetModel<GameModel>();
            statistics = model.statistics;
            if (achievementToggle.isOn)
            {
                OpenAchievements(true);
            }
            else
            {
                OpenStatistics(true);
            }
        }

        private void OnEnable()
        {
            if (statistics == null) return;
            
            if (achievementToggle.isOn)
            {
                OpenAchievements(true);
            }
            else
            {
                OpenStatistics(true);
            }
        }

        public void OpenStatistics(bool value)
        {
            if (value)
            {
                RefreshStatistics();
            }
            statisticsRoot.SetActive(value);
        }

        public void OpenAchievements(bool value)
        {
            if (value)
            {
                RefreshAchievements();
            }
            achievementsRoot.SetActive(value);
        }

        private void RefreshStatistics()
        {
            statisticsContent.ClearChildren();

            Type type = typeof(StatisticsEntry);
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.TryGetAttribute(out StatisticsMetadataAttribute metadata))
                {
                    UIStatisticView view = Instantiate(statisticPrefab, statisticsContent);
                    PopulateView(field, metadata, type, view);
                }
            }
        }

        private void PopulateView(FieldInfo field, StatisticsMetadataAttribute metadata, Type type, UIStatisticView view)
        {
            if (field.FieldType == typeof(int))
            {
                if (!metadata.maxField.Equals(""))
                {
                    int value = (int)field.GetValue(statistics.current);
                    FieldInfo maxField = type.GetField(metadata.maxField, BindingFlags.Instance | BindingFlags.Public);
                    int max = (int)maxField.GetValue(statistics.current);
                    view.DisplayProgress(metadata.name, value, max);
                }
                else
                {
                    int value = (int)field.GetValue(statistics.current);
                    view.DisplayStatistic(metadata.name, value, metadata.unit);
                }
            }
            else
            {
                float value = (float)field.GetValue(statistics.current);
                view.DisplayStatistic(metadata.name, value, metadata.unit);
            }
        }

        private void RefreshAchievements()
        {
            achievementContent.ClearChildren();
            foreach (Achievement achievement in achievements.GetAll())
            {
                bool isOpen = statistics.current.unlockedAchievements.IsUnlocked(achievement);
                UIAchievement achievementUI = Instantiate(achievementPrefab, achievementContent);
                
                if (achievement.hasProgress)
                {
                    ScriptableAchievementProvider provider = achievements.GetProvider(achievement);
                    Progress progress = provider.GetProgress(achievement, statistics.current);
                    achievementUI.SetAchievement(achievement, isOpen, progress.current, progress.max);
                }
                else
                {
                    achievementUI.SetAchievement(achievement, isOpen, 0, 0);
                }
            }
        }

    }
}
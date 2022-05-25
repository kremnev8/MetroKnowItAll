using System.Collections.Generic;
using System.Linq;
using Gameplay.MetroDisplay.Model;
using Gameplay.Statistics;
using Gameplay.UI;
using UnityEngine;
using Util;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Line Achievement Provider", menuName = "SO/New Line Achievement Provider", order = 0)]
    public class LineAchievementProvider : ScriptableAchievementProvider
    {
        public Metro metro;
        

        public override List<Achievement> GetAchievements()
        {
            List<Achievement> achievements = new List<Achievement>();

            foreach (MetroLine line in metro.lines)
            {
                string lineName = line.currentName
                    .Replace("ая", "ую")
                    .Replace("линия", "линию");

                Achievement achievement = new Achievement(
                    $"{baseId}_{line.lineId}", 
                    line.lineId, 
                    $"Открыть {lineName}", 
                    $"Откройте все {line.stations.Count} станций линии", 
                    true, 
                    baseIcon, 
                    line.lineColor
                    );
                achievements.Add(achievement);
            }

            return achievements;
        }

        public override Progress GetProgress(Achievement achievement, StatisticsEntry entry)
        {
            if (achievement.id.Contains(baseId))
            {
                MetroLine line = metro.lines[achievement.metadata];
                int unlocked = line.stations.Count(station => entry.unlockedStations.IsUnlocked(station));
                return new Progress(unlocked, line.stations.Count);
            }
            throw new WrongProviderException();
        }
        

        public override void CheckAchievementsState(AchievementDB achievements, StatisticsEntry entry)
        {
            foreach (MetroLine line in metro.lines)
            {
                string id = $"{baseId}_{line.lineId}";
                Achievement achievement = achievements.Get(id);
                if (!entry.unlockedAchievements.IsUnlocked(achievement))
                {
                    int unlocked = line.stations.Count(station => entry.unlockedStations.IsUnlocked(station));
                    if (unlocked >= line.stations.Count)
                    {
                        UIAchievementPopup.UnlockAchievement(id);
                    }
                }
            }
        }
    }
}
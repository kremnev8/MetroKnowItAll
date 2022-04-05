using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

namespace Gameplay.Questions.Model
{
    public class OrderStationsGenerator : StationQuestionGenerator<UIQuestionOrderStations>
    {
        public List<MetroStation> currentQuestionStations = new List<MetroStation>();

        public int currentCount;

        public List<int> tipBlacklisted = new List<int>();

        public override void GenerateNew()
        {
            currentCount = Random.Range(3, 7);
            currentQuestionStations.Clear();
            tipBlacklisted.Clear();
            
            for (int i = 0; i < currentCount; i++)
            {
                MetroStation next = metro.PickRandomStation(currentLineId, blacklistedIds);

                currentQuestionStations.Add(next);
                blacklistedIds.Add(next.globalId);
            }
            
            renderer.HideAllLabels();
            uiController.SetQuestion(currentQuestionStations);
            
            currentQuestionStations.Sort((station1, station2) => station1.globalId.CompareTo(station2.globalId));
            
            if (currentLineId != -1 && currentRegionType == RegionType.GLOBAL)
            {
                renderer.FocusLine(currentLineId);
            }
        }

        public override string GenerateTip(int tipNumber)
        {
            if (currentQuestionStations.Count < 2) return "";

            switch (tipNumber)
            {
                case 0:
                case 1 when currentQuestionStations.Count > 3:
                    int tipIndex = tipBlacklisted.ConstrainedRandom(0, currentQuestionStations.Count);
                    int nextIndex = tipIndex + 1;
                    if (nextIndex >= currentQuestionStations.Count)
                    {
                        nextIndex = tipIndex;
                        tipIndex -= 1;
                    }

                    MetroStation firstStation = metro.GetStation(currentLineId, currentQuestionStations[tipIndex].stationId);
                    MetroStation secondStation = metro.GetStation(currentLineId, currentQuestionStations[nextIndex].stationId);
                    
                    tipBlacklisted.Add(tipIndex);
                    tipBlacklisted.Add(nextIndex);

                    return $"Станция {firstStation.currentName} находится перед станцией {secondStation.currentName}";
            }

            return "";
        }

        public override bool ValidateAnswer()
        {
            List<MetroStation> selection = uiController.CurrentSelection();
            
            List<bool> correctnessList = currentQuestionStations.Select((t, i) => t.globalId == selection[i].globalId).ToList();
            List<bool> reversedList = Enumerable.Reverse(currentQuestionStations).Select((t, i) => t.globalId == selection[i].globalId).ToList();

            int correct = correctnessList.Count(b => b);
            int reversedCorrect = reversedList.Count(b => b);
            if (reversedCorrect > correct)
            {
                correctnessList = reversedList;
                correct = reversedCorrect;
            }

            bool allCorrect = correct == correctnessList.Count;
            
            uiController.DisplayResult(correctnessList, allCorrect);

            return allCorrect;
        }
    }
}
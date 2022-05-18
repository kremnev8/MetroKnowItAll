using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Conrollers;
using Gameplay.Core;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Questions.Generators
{
    /// <summary>
    /// This generator generates questions like: Order these stations on the map
    /// </summary>
    public class OrderStationsGenerator : QuestionGenerator<UIQuestionOrderStations>
    {
        public List<MetroStation> currentQuestionStations = new List<MetroStation>();

        public int currentCount;

        public List<int> tipBlacklisted = new List<int>();
        
        public const string QUESTION_ID = "station-order";
        public override string questionId => QUESTION_ID;

        public override void GenerateNew()
        {
            currentCount = Random.Range(3, 7);
            currentQuestionStations.Clear();
            tipBlacklisted.Clear();

            bool selected = false;

            while (!selected)
            {
                try
                {
                    currentQuestionStations = metro.PickRandomStationRange(currentRegion, currentCount, blacklistedIds);
                    selected = true;
                }
                catch (Exception e)
                {
                    Debug.Log($"Failed to generate, Reg: {currentRegion}, count: {currentCount}, blacklist: {string.Join(", ", blacklistedIds)}");
                    currentCount--;
                    if (currentCount < 3)
                    {
                        throw new Exception("Failed to generate Order question!");
                    }
                }
            }

            blacklistedIds.AddRange(currentQuestionStations.Select(station => (int)station.globalId));

            List<MetroStation> shuffled = new List<MetroStation>(currentQuestionStations);
            shuffled.Shuffle();
            uiController.SetQuestion(shuffled);

            renderer.FocusRegion(currentRegion);
            renderer.HideAllLabels();
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

                    MetroStation firstStation = currentQuestionStations[tipIndex];
                    MetroStation secondStation = currentQuestionStations[nextIndex];

                    Vector2 dir = secondStation.position - firstStation.position;
                    string dirName = dir.GetDirection();
                    
                    tipBlacklisted.Add(tipIndex);
                    tipBlacklisted.Add(nextIndex);

                    return $"Станция {firstStation.currentName} находится {dirName} станции {secondStation.currentName}";
            }

            return "";
        }

        public override bool ValidateAnswer()
        {
            List<MetroStation> selection = uiController.CurrentSelection();
            
            List<bool> correctnessList = currentQuestionStations.Select((t, i) => t.globalId == selection[i].globalId).ToList();
            int correct = correctnessList.Count(b => b);

            bool allCorrect = correct == correctnessList.Count;
            
            uiController.DisplayResult(correctnessList, allCorrect);
            for (int i = 0; i < currentQuestionStations.Count; i++)
            {
                MetroStation station = currentQuestionStations[i];
                if (station.globalId == selection[i].globalId)
                {
                    Simulation.GetModel<GameModel>().statistics.TryUnlockStation(station);
                }
            }

            return allCorrect;
        }
    }
}
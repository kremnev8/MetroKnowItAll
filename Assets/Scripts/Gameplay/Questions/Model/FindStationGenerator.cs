using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Questions.Model
{
    public class FindStationGenerator : StationQuestionGenerator<UIQuestionFindStation>
    {
        public MetroStation currentQuestion;

        public override void GenerateNew()
        {
            currentQuestion = metro.PickRandomStation(currentLineId, blacklistedIds);
            
            uiController.SetQuestion(currentQuestion);
            renderer.HideAllLabels();
            blacklistedIds.Add(currentQuestion.globalId);
            
            if (currentLineId != -1 && currentRegionType == RegionType.GLOBAL)
            {
                renderer.FocusLine(currentLineId);
            }
        }

        public override string GenerateTip(int tipNumber)
        {
            switch (tipNumber)
            {
                case 0:
                    MetroStation near = metro.PickStationNear(currentQuestion);
                    blacklistedIds.Add(near.globalId);
                    renderer.getStationDisplay(near).ShowLabelFor(Color.black, 1000);
                    return $"Станция метро {near.currentName} находиться рядом!";
                
            }

            return "";
        }

        public override bool ValidateAnswer()
        {
            MetroStation selectedStation = uiController.CurrentSelection();

            bool result = currentQuestion.globalId == selectedStation.globalId;
            uiController.DisplayResult(result);

            blacklistedIds.Add(selectedStation.globalId);
            return result;
        }
    }
}
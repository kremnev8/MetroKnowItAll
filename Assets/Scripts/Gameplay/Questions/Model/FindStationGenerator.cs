using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Questions.Model
{
    public class FindStationGenerator : QuestionGenerator<UIQuestionFindStation>
    {
        public MetroStation currentQuestion;

        public override void GenerateNew()
        {
            currentQuestion = metro.PickRandomStation(currentRegion, blacklistedIds);
            
            uiController.SetQuestion(currentQuestion);
            renderer.HideAllLabels();
            blacklistedIds.Add(currentQuestion.globalId);
            

            renderer.FocusLine(currentRegion);
        }

        public override string GenerateTip(int tipNumber)
        {
            switch (tipNumber)
            {
                case 0 when currentRegion.regionType == RegionType.GLOBAL:
                    MetroStation near = metro.PickStationNear(currentQuestion);
                    blacklistedIds.Add(near.globalId);
                    renderer.getStationDisplay(near).ShowLabelFor(GameController.theme.textColor, 1000);
                    return $"Станция метро {near.currentName} находиться рядом!";
                case 0 when currentRegion.regionType != RegionType.GLOBAL:
                    MetroLine line = metro.lines[currentQuestion.lineId];
                    //renderer.getStationDisplay(near).ShowLabelFor(GameController.theme.textColor, 1000);
                    return $"Станция принадлежит {line.name}!";
                
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
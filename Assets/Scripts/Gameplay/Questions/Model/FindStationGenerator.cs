using System;
using Gameplay.Conrollers;
using Gameplay.Core;
using Gameplay.MetroDisplay.Model;

namespace Gameplay.Questions.Generators
{
    /// <summary>
    /// This generator generates questions like: Where is that station on the map
    /// </summary>
    public class FindStationGenerator : QuestionGenerator<UIQuestionFindStation>
    {
        public MetroStation currentQuestion;

        public override string questionId => "find-station";

        public override void GenerateNew()
        {
            currentQuestion = metro.PickRandomStation(currentRegion, blacklistedIds);
            
            uiController.SetQuestion(currentQuestion);
            blacklistedIds.Add(currentQuestion.globalId);
            

            renderer.FocusRegion(currentRegion);
            renderer.HideAllLabels();
        }

        public override string GenerateTip(int tipNumber)
        {
            switch (tipNumber)
            {
                case 0:
                    MetroStation near = metro.PickStationNear(currentQuestion);
                    blacklistedIds.Add(near.globalId);
                    renderer.GetStationDisplay(near).ShowLabelFor(GameController.theme.textColor, 1000);
                    return $"Станция метро {near.currentName} находиться рядом!";
            }

            return "";
        }

        public override bool ValidateAnswer()
        {
            MetroStation selectedStation = uiController.CurrentSelection();

            bool result = currentQuestion.globalId == selectedStation.globalId;
            if (!result && selectedStation.currentName.Equals(currentQuestion.currentName, StringComparison.OrdinalIgnoreCase))
            {
                result = true;
            }
            
            if (result) 
                Simulation.GetModel<GameModel>().statistics.TryUnlockStation(currentQuestion);
            
            uiController.DisplayResult(result);


            blacklistedIds.Add(selectedStation.globalId);
            return result;
        }
    }
}
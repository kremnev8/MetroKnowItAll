using Gameplay.Conrollers;
using Gameplay.MetroDisplay.Model;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace Gameplay.Questions.Generators
{
    /// <summary>
    /// This generator generates questions like: Where is that line on the map
    /// </summary>
    public class FindLineGenerator : QuestionGenerator<UIQuestionFindLine>
    {
        public MetroLine currentQuestion;
        
        public MetroStation questionStation;
        public bool questionUsedLineName;
        
        public override string questionId => "find-line";
        
        public override void GenerateNew()
        {
            questionUsedLineName = Random.Range(0, 2) > 0;
            if (currentRegion.regionType == RegionType.GLOBAL_LINE && currentRegion.lineId != -1)
            {
                currentQuestion = metro.lines[currentRegion.lineId];
            }
            else
            {
                currentQuestion = metro.PickRandomLine(currentRegion);
            }

            if (questionUsedLineName)
            {
                questionStation = metro.PickRandomStation(new Region(currentRegion.regionType, currentRegion.stations, currentQuestion.lineId));
                uiController.SetQuestion(questionStation);
            }
            else
            {
                uiController.SetQuestion(currentQuestion);
            }

            renderer.FocusRegion(currentRegion);
            renderer.HideAllLabels();
        }

        public override string GenerateTip(int tipNumber)
        {
            switch (tipNumber)
            {
                case 0 when questionUsedLineName:
                    return $"Имя линии - {currentQuestion.name}";
                
                case 0 when !questionUsedLineName:
                case 1:
                case 2:
                    MetroStation station = metro.PickRandomStation(new Region(currentRegion.regionType, currentRegion.stations, currentQuestion.lineId));
                    return $"Станиция {station.currentName} распологается на этой линий";
            }

            return "";
        }

        public override bool ValidateAnswer()
        {
             MetroLine line = uiController.CurrentSelection();
             
             bool result = currentQuestion.lineId == line.lineId;
             uiController.DisplayResult(result);

             return result;
        }

        public override bool ShouldUse(Game game, int questionNumber)
        {
            if (game.mode == ArcadeModeController.MODE_ID && game.currentRegion.lineId == -1 && questionNumber == 0)
            {
                return base.ShouldUse(game, questionNumber);
            }

            return false;
        }
    }
}
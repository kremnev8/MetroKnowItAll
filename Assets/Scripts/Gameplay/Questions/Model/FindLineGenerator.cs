using System;
using Gameplay.Conrollers;
using Gameplay.MetroDisplay.Model;
using Random = UnityEngine.Random;

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
        
        public const string QUESTION_ID = "find-line";
        public override string questionId => QUESTION_ID;
        
        public override void GenerateNew()
        {
            if (metro.GetUnlockedLinesCount(currentRegion) <= 1)
            {
                throw new ArgumentException("Failed to generate Line question. Not enough lines unlocked!");
            }
            
            questionUsedLineName = Random.Range(0, 2) > 0;
            currentQuestion = metro.PickRandomLine(currentRegion, blacklistedIds);

            if (questionUsedLineName)
            {
                questionStation = metro.PickRandomStation(new Region(RegionType.GLOBAL_LINE, currentRegion.stations, currentQuestion.lineId));
                uiController.SetQuestion(questionStation);
            }
            else
            {
                uiController.SetQuestion(currentQuestion);
            }
            blacklistedIds.Add(currentQuestion.lineId);

            renderer.FocusRegion(currentRegion);
            renderer.HideAllLabels();
        }

        public override string GenerateTip(int tipNumber)
        {
            switch (tipNumber)
            {
                case 0 when questionUsedLineName:
                    return $"Имя линии - {currentQuestion.currentName}";
                
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
            if (game.currentRegion.lineId == -1)
            {
                return base.ShouldUse(game, questionNumber);
            }

            return false;
        }
    }
}

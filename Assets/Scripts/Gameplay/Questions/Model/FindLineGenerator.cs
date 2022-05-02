using Gameplay.MetroDisplay.Model;

namespace Gameplay.Questions.Model
{
    /// <summary>
    /// This generator generates questions like: Where is that line on the map
    /// </summary>
    public class FindLineGenerator : QuestionGenerator<UIQuestionFindLine>, ILineQuestion
    {
        public MetroLine currentQuestion;
        
        public override void GenerateNew()
        {
            if (currentRegion.regionType == RegionType.GLOBAL && currentRegion.lineId != -1)
            {
                currentQuestion = metro.lines[currentRegion.lineId];
            }
            else
            {
                currentQuestion = metro.PickRandomLine(currentRegion);
            }
            
            uiController.SetQuestion(currentQuestion);
            
            renderer.FocusRegion(currentRegion);
            renderer.HideAllLabels();
        }

        public override string GenerateTip(int tipNumber)
        {
            switch (tipNumber)
            {
                case 0:
                case 1:
                case 2:
                    MetroStation station = metro.PickRandomStation(new Region(RegionType.GLOBAL, Area.Everywhere, currentQuestion.lineId));
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
    }
}
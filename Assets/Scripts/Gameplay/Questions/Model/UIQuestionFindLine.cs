using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using UnityEngine;

namespace Gameplay.Questions.Model
{
    public class UIQuestionFindLine : BaseUIQuestion
    {
        public TouchButton button;
        
        public override BaseQuestionGenerator GetController()
        {
            return new FindLineGenerator();
        }

        public void SetQuestion(MetroLine line)
        {
            questionLabel.text = $"Укажи где находится \n{line.name}";
            bottomPane.sizeDelta = new Vector2(bottomPane.sizeDelta.x, 300);
            button.Enable(selectable => selectable is LineSubDisplay);
        }
        
        public void DisplayResult(bool result)
        {
            button.Disable();
        }

        public MetroLine CurrentSelection()
        {
            return button.GetSelected<LineSubDisplay>().line;
        }
    }
}
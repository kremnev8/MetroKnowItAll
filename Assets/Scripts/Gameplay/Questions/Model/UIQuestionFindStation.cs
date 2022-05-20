using System;
using Gameplay.Conrollers;
using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using UnityEngine;

namespace Gameplay.Questions.Generators
{
    /// <summary>
    /// UI for <see cref="FindStationGenerator"/>
    /// </summary>
    public class UIQuestionFindStation : BaseUIQuestion
    {
        public TouchButton button;

        public override BaseQuestionGenerator GetController()
        {
            return new FindStationGenerator();
        }


        public void SetQuestion(MetroStation station)
        {
            questionLabel.text = $"Укажи где находится станция\n{station.currentName}";
            bottomPane.sizeDelta = new Vector2(bottomPane.sizeDelta.x, 300);
            button.Enable(selectable => selectable is StationDisplay);
        }

        public void DisplayResult(bool result)
        {
            StationDisplay display = renderer.GetPrimaryDisplay(button.GetSelected<StationDisplay>());
            display.ShowLabelFor(result ? GameController.theme.rightAnswer : GameController.theme.wrongAnswer, 120);
            button.Disable();
        }

        public MetroStation CurrentSelection()
        {
            return button.GetSelected<StationDisplay>().station;
        }
        
    }
}
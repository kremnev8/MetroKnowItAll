using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Questions.Model;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Questions
{
    public class UIQuestionFindStation : BaseUIQuestion
    {
        private int correctGuesses;
        private int hideFeedbackIn;

        public TouchButton button;
        
        public TMP_Text questionLabel;
        public TMP_Text correctLabel;
        public TMP_Text feedbackLabel;

        public override BaseQuestionGenerator GetController()
        {
            return new FindStationGenerator();
        }
        
        
        public void SetQuestion(MetroStation station)
        {
            questionLabel.text = $"Укажи где находится станция\n{station.currnetName}";
        }

        public void DisplayResult(bool result)
        {
            if (result)
            {
                correctGuesses++;
                correctLabel.text = correctGuesses.ToString();
                feedbackLabel.text = "Верно!";
                button.selectedStation.SetLabelVisible(true, Color.green, false);
            }
            else
            {
                feedbackLabel.text = "Неправильно!";
                button.selectedStation.SetLabelVisible(true, Color.red, false);
            }
            button.HideSelector();

            hideFeedbackIn = 120;
        }

        public MetroStation CurrentSelection()
        {
            return button.selectedStation.station;
        }
        

        private void Update()
        {
            if (hideFeedbackIn > 0)
            {
                hideFeedbackIn--;
                if (hideFeedbackIn == 0)
                {
                    feedbackLabel.text = "";
                }
            }
        }
    }
}
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
        private int hideFeedbackIn;

        public TouchButton button;

        public RectTransform bottomPane;
        
        public TMP_Text questionLabel;
        public TMP_Text feedbackLabel;

        public override BaseQuestionGenerator GetController()
        {
            return new FindStationGenerator();
        }

        public override void HideElements()
        {
            questionLabel.text = "";
            button.gameObject.SetActive(false);
        }


        public void SetQuestion(MetroStation station)
        {
            button.gameObject.SetActive(true);
            questionLabel.text = $"Укажи где находится станция\n{station.currentName}";
            bottomPane.sizeDelta = new Vector2(bottomPane.sizeDelta.x, 300);
        }

        public void DisplayResult(bool result)
        {
            if (result)
            {
               // feedbackLabel.text = "Верно!";
                button.selectedStation.ShowLabelFor(PaletteHelper.theme.rightAnswer, 120);
            }
            else
            {
                //feedbackLabel.text = "Неправильно!";
                button.selectedStation.ShowLabelFor(PaletteHelper.theme.wrongAnswer, 120);
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
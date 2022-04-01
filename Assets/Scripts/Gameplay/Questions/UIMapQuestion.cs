using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Questions
{
    public class UIMapQuestion : MonoBehaviour
    {
        public List<string> nameList = new List<string>();
        
        private int currentQuestion;
        private int correctGuesses;

        private int hideFeedbackIn;
        
        public Metro metro;

        public TMP_Text questionLabel;
        public TMP_Text correctLabel;
        public TMP_Text feedbackLabel;
        
        private void Awake()
        {
            foreach (MetroLine line in metro.lines)
            {
                foreach (MetroStation station in line.stations) 
                {
                    if (!nameList.Any(s => station.currnetName.Contains(s)))
                    {
                        nameList.Add(station.currnetName);
                    }
                }
            }
            
            GenerateNewQuestion();
        }

        public void GenerateNewQuestion()
        {
            currentQuestion = Random.Range(0, nameList.Count);
            questionLabel.text = $"Укажи где находится станция\n{nameList[currentQuestion]}";
        }

        public bool CheckAnswer(MetroStation station)
        {
            string answer = nameList[currentQuestion];
            bool result = answer.Contains(station.currnetName);

            if (result)
            {
                correctGuesses++;
                correctLabel.text = correctGuesses.ToString();
                feedbackLabel.text = "Верно!";
            }
            else
            {
                feedbackLabel.text = "Неправильно!";
            }

            GenerateNewQuestion();
            hideFeedbackIn = 120;
            return result;
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
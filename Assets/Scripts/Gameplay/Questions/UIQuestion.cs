using System;
using System.Linq;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Char;

namespace Gameplay.Questions
{
    public class UIQuestion : MonoBehaviour
    {
        public TMP_Text questionText;
        public TMP_InputField answerField;

        public TMP_Text resultText;

        public Slider progressBar;

        public QuestionsDB questions;

        private Question currentQuestion;
        private int currentIndex;

        // Start is called before the first frame update
        void Start()
        {
            progressBar.maxValue = questions.Count();
            DisplayQuestion(0);
            resultText.text = "";
        }

        private void DisplayQuestion(int index)
        {
            if (index < questions.Count())
            {
                currentQuestion = questions.GetAll()[index];
                currentIndex = index;

                questionText.text = currentQuestion.QuestionText;
                (answerField.placeholder as TMP_Text).text = String.Concat(currentQuestion.QuestionAnswer.Select(HideAnswer));
                progressBar.value = currentIndex;
            }
        }

        private char HideAnswer(char c)
        {
            if (c != '-' && c != ' ')
            {
                return IsLower(c) ? 'x' : 'X';
            }

            return c;
        }

        public void CheckAnswer()
        {
            string text = answerField.text;
            if (text.Equals(currentQuestion.QuestionAnswer))
            {
                resultText.text = "Верный ответ!";
                DisplayQuestion(currentIndex + 1);
            }
            else
            {
                resultText.text = "Неверный ответ!";
            }
        }
    }
}
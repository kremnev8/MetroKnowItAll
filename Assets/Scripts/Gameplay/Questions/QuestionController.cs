using System;
using System.Collections.Generic;
using Model;
using Platformer.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Questions
{
    public class QuestionController : MonoBehaviour
    {
        public List<BaseUIQuestion> questionUI = new List<BaseUIQuestion>();
        public List<BaseQuestionGenerator> questionGenerators = new List<BaseQuestionGenerator>();

        
        private new MetroRenderer renderer;
        
        private int currentController;

        private Region currentRegion;
        private int questionsRemain;
        public int questionsPerRegion;
        

        private void Start()
        {
            renderer = Simulation.GetModel<GameModel>().renderer;
            
            foreach (BaseUIQuestion uiQuestion in questionUI)
            {
                BaseQuestionGenerator generator = uiQuestion.GetController();
                generator.Init(renderer, uiQuestion);
                questionGenerators.Add(generator);
            }
            SelectNextRegion();
            SelectNextController();
        }

        public void CheckAnswer()
        {
            questionGenerators[currentController].ValidateAnswer();

            questionsRemain--;
            if (questionsRemain == 0)
            {
                SelectNextRegion();
            }
            SelectNextController();
        }

        public void SelectNextController()
        {
            currentController = Random.Range(0, questionGenerators.Count);
            BaseQuestionGenerator generator = questionGenerators[currentController];
            
            generator.GenerateNew();
        }

        public void SelectNextRegion()
        {
            int lineId = Random.Range(0, renderer.metro.lines.Count);
            currentRegion.lineId = lineId;
            questionsRemain = questionsPerRegion;

            foreach (BaseQuestionGenerator generator in questionGenerators)
            {
                generator.SetRegion(currentRegion);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Platformer.Core;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Questions
{
    public class QuestionController : MonoBehaviour
    {
        public Transform uiQuestionTransfrom;
        
        [HideInInspector]
        public List<BaseUIQuestion> questionUI = new List<BaseUIQuestion>();
        public List<BaseQuestionGenerator> questionGenerators = new List<BaseQuestionGenerator>();

        
        private new MetroRenderer renderer;
        
        private int currentController;

        private Region currentRegion;
        private int questionsRemain;
        public int questionsPerRegion;

        public static Action onQuestionChanged;
        

        private void Start()
        {
            renderer = Simulation.GetModel<GameModel>().renderer;

            questionUI = uiQuestionTransfrom.GetComponents<BaseUIQuestion>().ToList();
            
            foreach (BaseUIQuestion uiQuestion in questionUI)
            {
                BaseQuestionGenerator generator = uiQuestion.GetController();
                generator.Init(renderer, uiQuestion);
                uiQuestion.renderer = renderer;
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
            
            Invoke(nameof(SelectNextController), 1.5f);
        }

        public void SelectNextController()
        {
            questionUI[currentController].HideElements();
            
            currentController = Random.Range(0, questionGenerators.Count);
            BaseQuestionGenerator generator = questionGenerators[currentController];
            try
            {
                generator.GenerateNew();
            }
            catch (Exception e)
            {
                SelectNextRegion();
                SelectNextController();
                return;
            }
            
            onQuestionChanged?.Invoke();
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

        public string GetNextTip(int index)
        {
            BaseQuestionGenerator generator = questionGenerators[currentController];
            return generator.GenerateTip(index);
        }
    }
}
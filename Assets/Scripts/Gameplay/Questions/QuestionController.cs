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
        public UITopBar topBar;
        
        [HideInInspector]
        public List<BaseUIQuestion> questionUI = new List<BaseUIQuestion>();
        public List<BaseQuestionGenerator> questionGenerators = new List<BaseQuestionGenerator>();

        private GameModel model;
        private new MetroRenderer renderer;

        private int currentController;

        private Region currentRegion;
        
        private int questionsRemain;
        public int questionsPerRegion;
        public int correctGuesses;

        public float timeout = 10;

        public static Action onQuestionChanged;

        public bool isResting;

        public float AnswerTimeElapsed;
        

        private void Start()
        {
            model = Simulation.GetModel<GameModel>();
            renderer = model.renderer;

            questionUI = uiQuestionTransfrom.GetComponents<BaseUIQuestion>().ToList();
            
            foreach (BaseUIQuestion uiQuestion in questionUI)
            {
                BaseQuestionGenerator generator = uiQuestion.GetController();
                generator.Init(renderer, uiQuestion);
                uiQuestion.renderer = renderer;
                questionGenerators.Add(generator);
            }
            Invoke(nameof(StartNextRegionTimer), 1f);
        }

        public void CheckAnswer()
        {
            if (isResting) return;
            
            bool result = questionGenerators[currentController].ValidateAnswer();

            if (result)
            {
                model.statistics.OnCorrectAnswer(AnswerTimeElapsed);
                correctGuesses++;
            }
            else
            {
                model.statistics.OnWrongAnswer();
            }
            questionsRemain--;
            
            topBar.UpdateStatus(result, correctGuesses, questionsPerRegion - questionsRemain);
            
            if (questionsRemain == 0)
            {
                Invoke(nameof(StartNextRegionTimer), 1.5f);
                return;
            }
            
            Invoke(nameof(SelectNextController), 1.5f);
        }

        public void SelectNextController()
        {
            questionUI[currentController].HideElements();

            var generators = questionGenerators.Where(questionGenerator =>
            {
                if (questionsRemain == questionsPerRegion && currentRegion.regionType != RegionType.GLOBAL)
                {
                    return questionGenerator is ILineQuestion;
                }
                    
                return !(questionGenerator is ILineQuestion);
            }).ToList();
            
            int index = Random.Range(0, generators.Count);
            BaseQuestionGenerator generator = generators[index];
            currentController = questionGenerators.IndexOf(generator);
            
            try
            {
                generator.GenerateNew();
            }
            catch (Exception e)
            {
                StartNextRegionTimer();
                Debug.Log($"{e.Message}\n{e.StackTrace}");
                return;
            }
            
            onQuestionChanged?.Invoke();
            AnswerTimeElapsed = 0;
        }

        private void StartNextRegionTimer()
        {
            if (isResting) return;
            
            isResting = true;
            questionUI[currentController].HideElements();
            if (correctGuesses == questionsPerRegion)
            {
                UIAchievement.UnlockAchievement("PerfectFive");
            }
            correctGuesses = 0;
            
            renderer.ClearFocus();
            renderer.ShowAllLabels();
            topBar.StartCountdown(timeout);
            Invoke(nameof(SelectNextRegion), timeout + 0.2f);
        }

        public void SelectNextRegion()
        {
            isResting = false;
            RegionType newType = (RegionType)Random.Range(0, (int) RegionType.MAX_VALUE);

            if (newType == RegionType.GLOBAL)
            {
                int lineId = Random.Range(0, renderer.metro.lines.Count);
                currentRegion = new Region(RegionType.GLOBAL, Area.Everywhere, lineId);
            }
            else
            {
                currentRegion = renderer.metro.regions.Find(region => region.regionType == newType);
            }

            Vector2 center = currentRegion.GetRegionCenter(renderer.metro);
            model.cameraController.LerpTo(center);
            
            questionsRemain = questionsPerRegion;

            foreach (BaseQuestionGenerator generator in questionGenerators)
            {
                generator.SetRegion(currentRegion);
            }
            
            topBar.SetCurrentLabel(currentRegion.GetName(renderer.metro));
            SelectNextController();
        }

        public string GetNextTip(int index)
        {
            BaseQuestionGenerator generator = questionGenerators[currentController];
            return generator.GenerateTip(index);
        }

        private void Update()
        {
            AnswerTimeElapsed += Time.deltaTime;
        }
    }
}
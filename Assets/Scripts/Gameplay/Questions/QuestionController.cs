using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using Gameplay.UI;
using Gameplay.Core;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Questions
{
    /// <summary>
    /// Selects and Manages question asking process. Contains all active <see cref="BaseQuestionGenerator"/> and <see cref="BaseUIQuestion"/>
    /// </summary>
    public class QuestionController : MonoBehaviour
    {
        public Transform uiQuestionTransfrom;
        public UITopBar topBar;
        
        [HideInInspector]
        public List<BaseUIQuestion> questionUI = new List<BaseUIQuestion>();
        public List<BaseQuestionGenerator> questionGenerators = new List<BaseQuestionGenerator>();

        public List<Region> allRegions = new List<Region>();

        private GameModel model;
        private new MetroRenderer renderer;

        private int currentController;

        private int currentRegionIndex;
        private Region currentRegion;
        
        private int questionsRemain;
        public int questionsPerRegion;

        public float timeout = 10;

        public static Action onQuestionChanged;

        public bool isResting;

        public float AnswerTimeElapsed;
        

        private void Start()
        {
            model = Simulation.GetModel<GameModel>();
            renderer = model.renderer;

            InitializeQuestions();
            InitializeRegions();
            Invoke(nameof(SelectNextRegion), 1f);
        }

        private void InitializeQuestions()
        {
            questionUI = uiQuestionTransfrom.GetComponents<BaseUIQuestion>().ToList();

            foreach (BaseUIQuestion uiQuestion in questionUI)
            {
                BaseQuestionGenerator generator = uiQuestion.GetController();
                generator.Init(renderer, uiQuestion);
                uiQuestion.renderer = renderer;
                questionGenerators.Add(generator);
            }
        }

        private void InitializeRegions()
        {
            allRegions.Clear();
            for (int i = 0; i < (int)RegionType.MAX_VALUE; i++)
            {
                RegionType regionType = (RegionType)i;
                if (regionType == RegionType.GLOBAL)
                {
                    foreach (MetroLine line in renderer.metro.lines)
                    {
                        allRegions.Add(new Region(RegionType.GLOBAL, Area.Everywhere, line.lineId));
                    }
                }
                else
                {
                    allRegions.Add(renderer.metro.regions.Find(region => region.regionType == regionType));
                }
            }
        }

        public void CheckAnswer()
        {
            if (isResting) return;

            bool result;
            try
            {
                 result = questionGenerators[currentController].ValidateAnswer(); 
            }
            catch (InvalidOperationException e)
            {
                topBar.ShowMessage("Выберете ответ!");
                return;
            }

            if (result)
            {
                model.statistics.OnCorrectAnswer(AnswerTimeElapsed);
            }
            else
            {
                model.statistics.OnWrongAnswer(AnswerTimeElapsed);
            }
            questionsRemain--;
            
            topBar.UpdateStatus(result, questionsPerRegion - questionsRemain, questionsPerRegion);
            
            if (questionsRemain == 0)
            {
                Invoke(nameof(SelectNextRegion), 1.5f);
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
                SelectNextRegion();
                Debug.Log($"{e.Message}\n{e.StackTrace}");
                return;
            }
            
            onQuestionChanged?.Invoke();
            AnswerTimeElapsed = 0;
        }

        private void SelectNextRegion()
        {
            if (isResting) return;
            
            isResting = true;
            questionUI[currentController].HideElements();

            currentRegion = allRegions.Where((region, i) => i != currentRegionIndex).RandomElementByWeight(region =>
            {
                return region.regionType == RegionType.GLOBAL ? 2 : 1;
            });
            currentRegionIndex = allRegions.IndexOf(currentRegion);
            
            Vector2 center = currentRegion.GetRegionCenter(renderer.metro);
            model.cameraController.LerpTo(center);
            
            renderer.ShowAllLabels();
            renderer.FocusRegion(currentRegion);
            
            topBar.StartCountdown(timeout);
            Invoke(nameof(ResumeGame), timeout + 0.2f);
        }

        public void ResumeGame()
        {
            isResting = false;

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
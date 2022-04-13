﻿using System;
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

        
        private new MetroRenderer renderer;

        private int currentController;

        private Region currentRegion;
        
        private int questionsRemain;
        public int questionsPerRegion;
        public int correctGuesses;

        public static Action onQuestionChanged;

        public bool isResting;
        

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
            Invoke(nameof(StartNextRegionTimer), 1f);
        }

        public void CheckAnswer()
        {
            if (isResting) return;
            
            bool result = questionGenerators[currentController].ValidateAnswer();

            if (result) correctGuesses++;
            questionsRemain--;
            
            topBar.UpdateStatus(result, correctGuesses, questionsPerRegion - questionsRemain);
            
            if (questionsRemain == 0)
            {
                StartNextRegionTimer();
                return;
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
                StartNextRegionTimer();
                Debug.Log($"{e.Message}\n{e.StackTrace}");
                return;
            }
            
            onQuestionChanged?.Invoke();
        }

        private void StartNextRegionTimer()
        {
            if (isResting) return;
            
            isResting = true;
            questionUI[currentController].HideElements();
            if (correctGuesses == questionsPerRegion)
            {
                topBar.DisplayAllCorrectMessage();
            }
            correctGuesses = 0;
            
            renderer.ClearFocus();
            renderer.ShowAllLabels();
            topBar.StartCountdown(10);
            Invoke(nameof(SelectNextRegion), 10.2f);
        }

        public void SelectNextRegion()
        {
            isResting = false;
            RegionType newType = (RegionType)Random.Range(0, (int) RegionType.MAX_VALUE);

            if (newType == RegionType.GLOBAL)
            {
                int lineId = Random.Range(0, renderer.metro.lines.Count);
                currentRegion.lineId = lineId;
                currentRegion.regionType = RegionType.GLOBAL;
                currentRegion.area = Area.Everywhere;
            }
            else
            {
                currentRegion = renderer.metro.regions.Find(region => region.regionType == newType);
            }
            
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
    }
}
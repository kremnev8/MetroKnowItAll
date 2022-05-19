using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using Gameplay.UI;
using Gameplay.Core;
using Gameplay.Questions;
using TMPro;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Conrollers
{
    /// <summary>
    /// Selects and Manages question asking process. Contains all active <see cref="BaseQuestionGenerator"/> and <see cref="BaseUIQuestion"/>
    /// </summary>
    public class ArcadeModeController : BaseGameMode
    {
        protected UIGame uiGame;
        protected Game game;
        protected GameModel model;
        protected new MetroRenderer renderer;
        protected GameModeController main;
        
        private List<Region> allRegions = new List<Region>();

        public const string MODE_ID = "arcade";
        public override string gameModeId => MODE_ID;

        public override void Init(GameModeController mainController)
        {
            model = Simulation.GetModel<GameModel>();
            renderer = model.renderer;
            uiGame = model.uiGame;
            main = mainController;
            InitializeRegions();
        }
        
        private void InitializeRegions()
        {
            allRegions.Clear();
            for (int i = 0; i < (int)RegionType.MAX_VALUE; i++)
            {
                RegionType regionType = (RegionType)i;
                if (regionType == RegionType.GLOBAL_LINE)
                {
                    foreach (MetroLine line in renderer.metro.lines)
                    {
                        allRegions.Add(new Region(RegionType.GLOBAL_LINE, line.lineId));
                    }
                }
                else
                {
                    allRegions.Add(renderer.metro.regions.Find(region => region.regionType == regionType));
                }
            }
        }

        public override void StartNewSession(Game gameState)
        {
            InitGameState(gameState);
            EventManager.TriggerEvent(EventTypes.SESSION_STARTED, game, true);
        }

        public override void ContinueSession(Game gameState)
        {
            InitGameState(gameState);
            EventManager.TriggerEvent(EventTypes.SESSION_STARTED, game, false);
        }
        protected void InitGameState(Game gameState)
        {
            game = gameState;
            game.currentQuestion = 0;

            game.attemptsLeft = model.settings.currentDifficulty.maxAttempts;
            game.partialAttempts = 0;
            game.questionId = Game.ANY_QUESTIONS;

            game.isPlaying = true;
            uiGame.topBar.SetCurrentAttemptsImmidiate(game.attemptsLeft, 0);
            Invoke(nameof(PrepareNewGame), 1f);
        }

        public virtual void SessionOver()
        {
            uiGame.answerPanelSwipe.ForceClosed();
            model.gameOverScreen.PopupArcade(model.statistics.sesion, model.statistics.sesionUnlockedStations);
            game.isPlaying = false;
            EventManager.TriggerEvent(EventTypes.SESSION_ENDED);
        }

        public override void ConfirmPressed()
        {
            if (!game.isPlaying) return;
            CheckAnswer();
        }

        public override void StartGamePressed()
        {
            ResumeGame();
            uiGame.DisableStartButton();
            uiGame.SetConfirmText("Подтвердить");
        }
        

        protected void CheckAnswer()
        {
            bool result;
            try
            {
                 result = main.GetGenerator(game.currentGenerator).ValidateAnswer(); 
            }
            catch (InvalidOperationException e)
            {
                uiGame.topBar.ShowMessage("Выберите ответ!");
                return;
            }

            game.currentQuestion++;
            uiGame.topBar.UpdateStatus(result, game.currentQuestion, game.maxQuestions);
            
            UpdateGameState(result);
            
            if (game.attemptsLeft < 0)
            {
                SessionOver();
                return;
            }
            
            EventManager.TriggerEvent(EventTypes.QUESTION_ANSWERED, result, game.answerTimeElapsed);

            if (game.currentQuestion >= game.maxQuestions)
            {
                Invoke(nameof(PrepareNewGame), 1.5f);
                return;
            }
            
            Invoke(nameof(SelectNextController), 1.5f);
        }

        protected virtual void UpdateGameState(bool result)
        {
            if (result)
            {
                if (game.attemptsLeft < model.settings.currentDifficulty.maxAttempts)
                {
                    game.partialAttempts += 2;
                    game.partialAttempts += model.statistics.sesion.correctAnswerStreak;
                }
            }
            else
            {
                game.attemptsLeft--;
            }

            int patialMax = model.settings.currentDifficulty.partialPerAttempt;
            int oldAttempts = game.attemptsLeft;
            float oldPartial = game.partialAttempts / (float)patialMax;

            if (game.partialAttempts > patialMax)
            {
                int attemptAdd = game.partialAttempts / patialMax;
                attemptAdd = Mathf.Min(model.settings.currentDifficulty.maxAttempts - game.attemptsLeft, attemptAdd);

                game.partialAttempts -= attemptAdd * game.partialAttempts;
                game.attemptsLeft += attemptAdd;
            }

            if (game.attemptsLeft >= 0)
            {
                uiGame.topBar.SetCurrentAttempts(oldAttempts, game.attemptsLeft, oldPartial);
            }
        }

        public void SelectNextController()
        {
            SelectNextController(-1);
        }

        public void SelectNextController(int exclude)
        {
            try
            {
                main.GetUI(game.currentGenerator).HideElements();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            main.SelectGenerator(game, exclude);
            BaseQuestionGenerator generator = main.GetGenerator(game.currentGenerator);
            
            try
            {
                generator.GenerateNew();
            }
            catch (Exception e)
            {
                SelectNextController(game.currentGenerator);
                Debug.Log($"{e.Message}\n{e.StackTrace}");
                return;
            }
            
            uiGame.answerPanelSwipe.ForceOpen();
            game.answerTimeElapsed = 0;
        }

        private void PrepareNewGame()
        {
            game.isPlaying = false;
            main.GetUI(game.currentGenerator).HideElements();
            uiGame.answerPanelSwipe.ForceClosed();
            
            game.currentRegion = SelectRegion();

            Vector2 center = game.currentRegion.GetRegionCenter(renderer.metro);
            if (!center.IsInfinity())
            {
                model.cameraController.LerpTo(center);
            }

            OnRestStarted();
        }

        protected virtual Region SelectRegion()
        {
            int regionIndex = Enumerable.Range(0, allRegions.Count)
                .Where((region) => !allRegions[region].Equals(game.currentRegion))
                .RandomElementByWeight(region => { return allRegions[region].regionType == RegionType.GLOBAL_LINE ? 2 : 1; });
            return allRegions[regionIndex];
        }

        protected virtual void OnRestStarted()
        {
            renderer.ShowAllLabels();
            renderer.FocusRegion(game.currentRegion);
            uiGame.EnableStartButton($"{game.maxQuestions} вопросов");
            uiGame.SetConfirmText("Подтвердить", false);
            uiGame.topBar.SetCurrentLabel("Перерыв");
        }

        public void ResumeGame()
        {
            game.isPlaying = true;
            
            game.currentQuestion = 0;
            game.correctAnswers = 0;

            foreach (BaseQuestionGenerator generator in main.questionGenerators)
            {
                generator.SetRegion(game.currentRegion);
            }
            
            uiGame.topBar.SetCurrentLabel(game.currentRegion.GetName(renderer.metro));
            SelectNextController();
        }

        public override string GetNextTip(int index)
        {
            if (game.isPlaying)
            {
                BaseQuestionGenerator generator = main.GetGenerator(game.currentGenerator);
                return generator.GenerateTip(index);
            }

            return "";
        }
        
        public override void ManualUpdate()
        {
            game.answerTimeElapsed += Time.deltaTime;
        }
    }
}
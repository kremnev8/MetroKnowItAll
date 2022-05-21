using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using Gameplay.UI;
using Gameplay.Core;
using Gameplay.Questions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Conrollers
{
    /// <summary>
    /// Selects and Manages question asking process. Contains all active <see cref="BaseQuestionGenerator"/> and <see cref="BaseUIQuestion"/>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
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

        [JsonProperty] [HideInInspector] public int maxQuestions;


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
                    allRegions.Add(new Region(regionType, -1));
                }
            }
        }

        public override void SetupNewSession(Game gameState)
        {
            uiGame.intro.ShowIntro(gameState);
            maxQuestions = 10;
        }

        public override void ContinueSession(Game gameState)
        {
            InitGameState(gameState);
            EventManager.TriggerEvent(EventTypes.SESSION_STARTED, game);
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
            Invoke(nameof(PrepareNewGame), 0.2f);
        }

        public virtual void SessionOver()
        {
            Save();
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
            uiGame.topBar.UpdateStatus(result, game.currentQuestion, maxQuestions);

            UpdateGameState(result);

            if (game.attemptsLeft < 0)
            {
                SessionOver();
                return;
            }

            EventManager.TriggerEvent(EventTypes.QUESTION_ANSWERED, result, game.answerTimeElapsed);

            if (game.currentQuestion >= maxQuestions)
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
            try
            {
                main.GetUI(game.currentGenerator).HideElements();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            bool questionSelected = false;
            HashSet<int> controllerBlacklist = new HashSet<int>();

            while (!questionSelected)
            {
                bool success = main.SelectGenerator(game, controllerBlacklist);
                if (!success)
                {
                    Invoke(nameof(PrepareNewGame), 1.5f);
                    return;
                }

                BaseQuestionGenerator generator = main.GetGenerator(game.currentGenerator);

                try
                {
                    generator.GenerateNew();
                    questionSelected = true;
                }
                catch (Exception e)
                {
                    controllerBlacklist.Add(game.currentGenerator);
                    Debug.Log($"Controller {generator.GetType().Name} failed to generate!\n{e.Message}\n{e.StackTrace}");
                }
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
            uiGame.EnableStartButton($"{maxQuestions} вопросов");
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

        public override void OnVersionChanged(int oldVersion) { }

        public override void ManualUpdate()
        {
            game.answerTimeElapsed += Time.deltaTime;
        }
    }
}
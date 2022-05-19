using System;
using System.Linq;
using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using Gameplay.Questions.Generators;
using Gameplay.Statistics;
using TMPro;
using UnityEngine;
using Util;

namespace Gameplay.Conrollers
{
    public class LearningModeController : ArcadeModeController
    {

        public new const string MODE_ID = "learning";
        public override string gameModeId => MODE_ID;

        public int minQuestions;
        public int maxQuestions;
        public int questionIncrement;

        public override ISaveController CreateModelHandler()
        {
            LearningSaveController saveController = gameObject.AddComponent<LearningSaveController>();
            return saveController;
        }

        public override void SetupNewSession(Game gameState)
        {
            GetModel<LearningSaveInterface>().tokens = 10;
            GetModel<LearningSaveInterface>().unlockedStations = new BoolRecord<MetroStation, int>();
            uiGame.intro.ShowIntro(gameState);
        }

        public override void ContinueSession(Game gameState)
        {
            InitGameState(gameState);
            game.questionId = FindStationGenerator.QUESTION_ID;
            EventManager.TriggerEvent(EventTypes.SESSION_STARTED, game);
        }

        protected override void OnRestStarted()
        {
            float score = game.correctAnswers * 0.2f;
            int gameMaxQuestions = game.maxQuestions;
            if (game.correctAnswers == game.maxQuestions)
            {
                score *= 2;
                int newMax = gameMaxQuestions + questionIncrement;
                if (GetModel<LearningSaveInterface>().unlockedStations.data.Count > newMax && newMax <= maxQuestions)
                {
                    game.maxQuestions = newMax;
                }
            }
            else
            {
                int newMax = gameMaxQuestions - questionIncrement;
                if (newMax >= minQuestions)
                {
                    game.maxQuestions = newMax;
                }
            }

            int tickets = Mathf.RoundToInt(score);
            GetModel<LearningSaveInterface>().tokens += tickets;

            if (game.correctAnswers > 0)
            {
                model.gameOverScreen.PopupLearning(game.correctAnswers, gameMaxQuestions, tickets);
            }

            base.OnRestStarted();
            Refresh();
            uiGame.SetConfirmText("Открыть", true);

            uiGame.touchButton.Enable(selectable =>
            {
                if (selectable is StationDisplay display)
                {
                    if (renderer.metro.IsStationAdjacent(display.station, GetModel<LearningSaveInterface>().unlockedStations.data) ||
                        GetModel<LearningSaveInterface>().unlockedStations.data.Count == 0)
                    {
                        return true;
                    }
                }

                return false;
            }, true, true);
        }
        
        public override void SessionOver()
        {
            uiGame.answerPanelSwipe.ForceClosed();
            game.isPlaying = false;
            EventManager.TriggerEvent(EventTypes.SESSION_ENDED);
        }

        public override void ConfirmPressed()
        {
            if (game.isPlaying)
            {
                base.ConfirmPressed();
            }
            else
            {
                Purchase();
            }
        }

        public override void StartGamePressed()
        {
            uiGame.touchButton.Disable();
            game.currentRegion = SelectRegion();
            base.StartGamePressed();
        }

        protected override Region SelectRegion()
        {
            return new Region(RegionType.GLOBAL_STATIONS, GetModel<LearningSaveInterface>().unlockedStations.data, -1);
        }

        protected override void UpdateGameState(bool result)
        {
            if (result)
            {
                game.correctAnswers++;
            }
        }

        public void Purchase()
        {
            StationDisplay display;
            try
            {
                display = uiGame.touchButton.GetSelected<StationDisplay>();
            }
            catch (Exception e)
            {
                uiGame.topBar.ShowMessage("Выберите станцию!");
                return;
            }
            
            if (display == null || GetModel<LearningSaveInterface>().tokens <= 0) return;

            if (!GetModel<LearningSaveInterface>().unlockedStations.IsUnlocked(display.station))
            {
                GetModel<LearningSaveInterface>().unlockedStations.Unlock(display.station);
                GetModel<LearningSaveInterface>().tokens -= 1;
                try
                {
                    MetroCrossing crossing =
                        renderer.metro.crossings.First(crossing => crossing.stationsGlobalIds.Contains(display.station.globalId));
                    foreach (GlobalId globalId in crossing.stationsGlobalIds)
                    {
                        if (globalId != display.station.globalId)
                        {
                            GetModel<LearningSaveInterface>().unlockedStations.Unlock(renderer.metro.GetStation(globalId));
                        }
                    }
                }
                catch (InvalidOperationException) { }

                Refresh();
            }
        }

        private void Refresh()
        {
            uiGame.topBar.UpdateTickets(GetModel<LearningSaveInterface>().tokens);
            game.currentRegion = SelectRegion();
            renderer.FocusRegion(game.currentRegion);
            if (GetModel<LearningSaveInterface>().unlockedStations.data.Count < 10)
            {
                int need = 10 - GetModel<LearningSaveInterface>().unlockedStations.data.Count;
                uiGame.EnableStartButton( $"Откройте {need} станций");
            }
            else
            {
                uiGame.EnableStartButton($"{game.maxQuestions} вопросов");
            }
            uiGame.SetStartInteractable(GetModel<LearningSaveInterface>().unlockedStations.data.Count > 10);
        }
    }
}
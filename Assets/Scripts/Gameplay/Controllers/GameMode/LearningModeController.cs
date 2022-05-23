﻿using System;
using System.Linq;
using Gameplay.Controls;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using Gameplay.Questions.Generators;
using Gameplay.Statistics;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Conrollers
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class LearningModeController : ArcadeModeController
    {
        public new const string MODE_ID = "learning";
        public override string gameModeId => MODE_ID;

        [SerializeField] protected Sprite tokenIcon;

        public int minQuestionsConfig;
        public int maxQuestionsConfig;
        public int questionIncrement;

        
        [JsonProperty]
        [HideInInspector]
        public int tokens;
        
        [JsonProperty]
        [HideInInspector]
        public BoolRecord<MetroStation, int> unlockedStations = new BoolRecord<MetroStation, int>();
        

        public override void SetupNewSession(Game gameState)
        {
            tokens = 10;
            unlockedStations = new BoolRecord<MetroStation, int>();
            maxQuestions = 10;
            uiGame.intro.ShowIntro(gameState);
        }

        public override void ContinueSession(Game gameState)
        {
            InitGameState(gameState);
            
            uiGame.topBar.SwitchTokensIcon(tokenIcon);
            game.questionId = FindStationGenerator.QUESTION_ID;
            renderer.year = targetMetro.startYear;
            EventManager.TriggerEvent(EventTypes.SESSION_STARTED, game);
        }

        protected override void OnRestStarted()
        {
            base.OnRestStarted();
            Refresh();
            uiGame.SetConfirmText("Открыть", true);

            uiGame.touchButton.Enable(selectable =>
            {
                if (selectable is StationDisplay display)
                {
                    if (renderer.metro.IsStationAdjacent(display.station, unlockedStations.data) ||
                        unlockedStations.data.Count == 0)
                    {
                        return true;
                    }
                }

                return false;
            }, true, true);
        }
        
        public override void SessionOver()
        {
            Save();
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
            return new Region(RegionType.GLOBAL_STATIONS, unlockedStations.data, -1);
        }

        protected override void UpdateGameState(bool result)
        {
            if (result)
            {
                game.correctAnswers++;
            }

            if (game.currentQuestion >= maxQuestions)
            {
                Invoke(nameof(CalculateTickets), 1.5f);
            }
        }

        private void CalculateTickets()
        {
            float score = game.correctAnswers * 0.2f;
            int gameMaxQuestions = maxQuestions;
            if (game.correctAnswers == maxQuestions)
            {
                score *= 2;
                int newMax = gameMaxQuestions + questionIncrement;
                if (unlockedStations.data.Count > newMax && newMax <= maxQuestionsConfig)
                {
                    maxQuestions = newMax;
                }
            }
            else
            {
                int newMax = gameMaxQuestions - questionIncrement;
                if (newMax >= minQuestionsConfig)
                {
                    maxQuestions = newMax;
                }
            }

            int tmp_tokens = Mathf.RoundToInt(score);
            tmp_tokens += tmp_tokens;

            model.gameOverScreen.PopupLearning(game.correctAnswers, gameMaxQuestions, tmp_tokens, GetTokenName(tmp_tokens));
        }

        public virtual string GetTokenName(int count)
        {
            return count.GetDeclension("билет", "билета", "билетов");
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
            
            if (display == null || tokens <= 0) return;

            if (!unlockedStations.IsUnlocked(display.station))
            {
                unlockedStations.Unlock(display.station);
                tokens -= 1;
                DetermineAdditionalUnlocks(display.station.globalId);
                
                if (game.correctAnswers == maxQuestions)
                {
                    int newMax = maxQuestions + questionIncrement;
                    if (unlockedStations.data.Count > newMax && newMax <= maxQuestionsConfig)
                    {
                        maxQuestions = newMax;
                    }
                }
                
                Refresh();
            }
        }

        public virtual void DetermineAdditionalUnlocks(GlobalId globalId)
        {
            try
            {
                MetroCrossing crossing =
                    renderer.metro.crossings.First(crossing => crossing.stationsGlobalIds.Contains(globalId));
                foreach (GlobalId globalId1 in crossing.stationsGlobalIds)
                {
                    if (globalId1 != globalId)
                    {
                        unlockedStations.Unlock(renderer.metro.GetStation(globalId));
                    }
                }
            }
            catch (InvalidOperationException) { }
        }

        protected virtual void Refresh()
        {
            uiGame.topBar.UpdateTokens(tokens);
            game.currentRegion = SelectRegion();
            renderer.FocusRegion(game.currentRegion);
            if (unlockedStations.data.Count < 10)
            {
                int need = 10 - unlockedStations.data.Count;
                uiGame.EnableStartButton( $"Откройте {need} станций");
                uiGame.SetStartInteractable(false);
            }else if (tokens > 10)
            {
                uiGame.EnableStartButton( "Откройте больше станций");
                uiGame.SetStartInteractable(false);
            }
            else
            {
                uiGame.EnableStartButton($"{maxQuestions} вопросов");
                uiGame.SetStartInteractable(true);
            }
            
        }
    }
}
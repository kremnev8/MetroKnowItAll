using System.Collections.Generic;
using System.Linq;
using Gameplay.MetroDisplay.Model;
using Gameplay.Questions.Generators;
using Newtonsoft.Json;
using UnityEngine;
using Util;

namespace Gameplay.Conrollers
{
    public class HistoricModeController : LearningModeController
    {
        public new const string MODE_ID = "historic";
        public override string gameModeId => MODE_ID;

        [JsonProperty]
        [HideInInspector] 
        public int currentYear;
        
        private bool readyForTransition;
        private int eventfulYearIndex;
        

        public override void SetupNewSession(Game gameState)
        {
            base.SetupNewSession(gameState);
            currentYear = targetMetro.startYear;
        }

        public override void ContinueSession(Game gameState)
        {
            InitGameState(gameState);
            
            uiGame.topBar.SwitchTokensIcon(tokenIcon);
            game.questionId = FindStationGenerator.QUESTION_ID;
            renderer.year = currentYear;
            eventfulYearIndex = targetMetro.GetEventfulYears().FindIndex(i => i == currentYear);
            CheckTransitionState();
            EventManager.TriggerEvent(EventTypes.SESSION_STARTED, game);
        }

        protected override void OnRestStarted()
        {
            base.OnRestStarted();
            uiGame.topBar.SetCurrentLabel($"Перерыв - {currentYear} г.");
            uiGame.SetConfirmText("Построить", true);
        }

        public override void StartGamePressed()
        {
            if (readyForTransition)
            {
                eventfulYearIndex++;
                currentYear = targetMetro.GetEventfulYears()[eventfulYearIndex];
                model.statistics.current.lastReachedYear = Mathf.Max(model.statistics.current.lastReachedYear, currentYear);
                renderer.year = currentYear;
                readyForTransition = false;
                Refresh();
            }
            else
            {
                base.StartGamePressed();
            }
        }

        protected override void Refresh()
        {
            base.Refresh();
            CheckTransitionState();
            uiGame.topBar.SetCurrentLabel($"Перерыв - {currentYear} г.");
        }

        private void CheckTransitionState()
        {
            bool allUnlocked = AllStationsUnlocked();
            if (allUnlocked)
            {
                List<int> eventfulYears = targetMetro.GetEventfulYears();
                if (eventfulYearIndex + 1 < eventfulYears.Count)
                {
                    int nextYear = eventfulYears[eventfulYearIndex + 1];
                    if (nextYear < 2022)
                    {
                        uiGame.EnableStartButton($"Перейти в год {nextYear}");
                        uiGame.SetStartInteractable(true);
                        readyForTransition = true;
                    }
                    else
                    {
                        int totalCount = 0;
                        foreach (MetroLine line in targetMetro.lines)
                        {
                            totalCount += line.stations.Count(station => station.isOpen);
                        }
                        model.gameOverScreen.PopupHistoric(totalCount, totalTokens, totalGames);
                        uiGame.DisableStartButton();
                    }
                }
            }
        }
        
        public override string GetTokenName(int count)
        {
            return count.GetDeclension("молоток", "молотка", "молотков");
        }

        public override void DetermineAdditionalUnlocks(GlobalId globalId)
        {
            base.DetermineAdditionalUnlocks(globalId);
            foreach (MetroSiblings sibling in targetMetro.siblingStations)
            {
                if (sibling.stations.Contains(globalId))
                {
                    foreach (GlobalId siblingStation in sibling.stations)
                    {
                        unlockedStations.Unlock(renderer.metro.GetStation(siblingStation));
                    }
                }
            }
        }

        private bool AllStationsUnlocked()
        {
            foreach (MetroLine line in targetMetro.lines)
            {
                if (line.IsOpen(currentYear))
                {
                    foreach (MetroStation station in line.stations)
                    {
                        if (station.isOpen &&
                            !unlockedStations.IsUnlocked(station))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
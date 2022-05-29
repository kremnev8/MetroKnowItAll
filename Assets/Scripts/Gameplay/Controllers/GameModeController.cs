using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using Gameplay.Questions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Conrollers
{
    /// <summary>
    /// Game modes controller class. Handles storing game mode instance classes, and activating current one. Contains all active <see cref="BaseQuestionGenerator"/> and <see cref="BaseUIQuestion"/>
    /// </summary>
    public class GameModeController : MonoBehaviour
    {
        [SerializeField] private Transform uiQuestionTransfrom;
        
        [HideInInspector]
        public List<BaseUIQuestion> questionUI = new List<BaseUIQuestion>();
        public List<BaseQuestionGenerator> questionGenerators = new List<BaseQuestionGenerator>();

        private Dictionary<string, BaseGameMode> gameModes = new Dictionary<string, BaseGameMode>();
        public string currentGameMode = "";
        
        private GameModel model;
        private new MetroRenderer renderer;
        private StatisticsController statistics;
        
        public Game gameState;
        
        private void Start()
        {
            model = Simulation.GetModel<GameModel>();
            renderer = model.renderer;
            statistics = model.statistics;

            InitializeQuestions();

            BaseGameMode[] gameModes1 = GetComponents<BaseGameMode>();
            foreach (BaseGameMode gameMode in gameModes1)
            {
                gameMode.Init(this);
                gameModes.Add(gameMode.gameModeId, gameMode);
            }
        }

        public void StartGame(int gameModeId)
        {
            switch (gameModeId)
            {
                case 0:
                    StartGame(ArcadeModeController.MODE_ID);
                    break;
                case 1:
                    StartGame(LearningModeController.MODE_ID);
                    break;
                case 2:
                    StartGame(HistoricModeController.MODE_ID);
                    break;
            }
        }

        public void StartGame(string gameModeId)
        {
            if (gameModes.ContainsKey(gameModeId))
            {
                BaseGameMode gameMode = gameModes[gameModeId];
                gameState.Reset();
                gameState.mode = gameModeId;
                currentGameMode = gameModeId;
                
                gameMode.StartSession(gameState);
            }
            
        }

        public void ConfirmPressed()
        {
            if (!currentGameMode.Equals("") && gameModes.ContainsKey(currentGameMode))
            {
                gameModes[currentGameMode].ConfirmPressed();
            }
        }

        public void StartGamePressed()
        {
            if (!currentGameMode.Equals("") && gameModes.ContainsKey(currentGameMode))
            {
                gameModes[currentGameMode].StartGamePressed();
            }
        }

        public void UpdateGenerators(Metro metro)
        {
            foreach (BaseQuestionGenerator generator in questionGenerators)
            {
                generator.metro = metro;
            }
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

        public BaseQuestionGenerator GetGenerator(int index)
        {
            if (index < questionGenerators.Count)
            {
                return questionGenerators[index];
            }

            throw new ArgumentException($"Index is out of range! index: {index}");
        }
        
        public BaseUIQuestion GetUI(int index)
        {
            if (index < questionUI.Count)
            {
                return questionUI[index];
            }

            throw new ArgumentException($"Index is out of range! index: {index}");
        }

        public bool SelectGenerator(Game game, HashSet<int> blacklist)
        {
            List<int> generators = Enumerable.Range(0, questionGenerators.Count)
                .Where(index =>
                {
                    if (blacklist.Contains(index)) return false;
                    return questionGenerators[index].ShouldUse(game, game.currentQuestion);
                })
                .ToList();

            if (generators.Count == 0)
            {
                return false;
            }
            
            int option = Random.Range(0, generators.Count);
            game.currentGenerator = generators[option];
            return true;
        }

        private void Update()
        {
            if (!currentGameMode.Equals("") && gameModes.ContainsKey(currentGameMode))
            {
                gameModes[currentGameMode].ManualUpdate();
            }
        }

        public string GetNextTip(int currentTip)
        {
            if (!currentGameMode.Equals("") && gameModes.ContainsKey(currentGameMode))
            {
                return gameModes[currentGameMode].GetNextTip(currentTip);
            }

            return "";
        }
    }
}
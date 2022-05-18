using System;
using Gameplay.Conrollers;
using UnityEngine;
using Util;

namespace Gameplay.UI
{
    public class UIIntro : MonoBehaviour
    {
        public GameObject arcadeCard;
        public GameObject learningCard;
        public GameObject historicCard;


        private void Awake()
        {
            EventManager.StartListening(EventTypes.SESSION_STARTED, OnNewGame);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventManager.StopListening(EventTypes.SESSION_STARTED, OnNewGame);
        }
        
        private void OnNewGame(object[] obj)
        {
            Game game = (Game)obj[0];
            bool newGame = (bool)obj[1];
            if (newGame)
            {
                arcadeCard.SetActive(game.mode == ArcadeModeController.MODE_ID);
                learningCard.SetActive(game.mode == LearningModeController.MODE_ID);
                historicCard.SetActive(false);

                gameObject.SetActive(true);
            }
        }
    }
}
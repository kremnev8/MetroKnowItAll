﻿using System;
using Gameplay.Conrollers;
using UnityEngine;
using Util;

namespace Gameplay.UI
{
    /// <summary>
    /// Controller for game mode intro UI
    /// </summary>
    public class UIIntro : MonoBehaviour
    {
        public GameObject arcadeCard;
        public GameObject learningCard;
        public GameObject historicCard;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void ShowIntro(Game game)
        {
            arcadeCard.SetActive(game.mode == ArcadeModeController.MODE_ID);
            learningCard.SetActive(game.mode == LearningModeController.MODE_ID);
            historicCard.SetActive(game.mode == HistoricModeController.MODE_ID);

            gameObject.SetActive(true);
        }
    }
}
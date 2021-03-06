using System;
using System.Collections.Generic;
using Gameplay.Conrollers;
using Gameplay.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay.Questions
{
    /// <summary>
    /// Controller for question tips
    /// </summary>
    public class UITipController : MonoBehaviour
    {
        public int maxTips;
        public Transform tipsRoot;

        public Transform buttonTransform;
        public Button button;
        public TMP_Text buttonLabel;
        
        public UITipContainer tipPrefab;
        
        
        private List<UITipContainer> tips = new List<UITipContainer>();
        private int currentTip;
        private GameModeController controller;
        
        private void Start()
        {
            currentTip = 0;
            controller = Simulation.GetModel<GameModel>().gameModeController;
            EventManager.StartListening(EventTypes.QUESTION_ANSWERED, OnAnswer);
            EventManager.StartListening(EventTypes.GAME_STARTED, OnAnswer); 
        }

        private void OnDestroy()
        {
            EventManager.StopListening(EventTypes.QUESTION_ANSWERED, OnAnswer);
            EventManager.StopListening(EventTypes.GAME_STARTED, OnAnswer);
        }
        
        private void OnAnswer(object[] obj)
        {
            Debug.Log("Answer!");
            ClearTips();
        }

        private void ClearTips()
        {
            foreach (UITipContainer tip in tips)
            {
                Destroy(tip.gameObject);
            }
            tips.Clear();
            
            buttonLabel.text = "Новая подсказка";
            button.interactable = true;

            currentTip = 0;
        } 

        public void NextTip()
        {
            if (!controller.gameState.isPlaying)
            {
                buttonLabel.text = "Подсказок нет. Начните игру!";
                button.interactable = false;
                return;
            }
            
            if (currentTip >= maxTips)
            {
                buttonLabel.text = "Подсказки кончились";
                button.interactable = false;
                return;
            }
            
            string tip = controller.GetNextTip(currentTip);
            if (tip.Equals("")) return;
            
            
            UITipContainer tipContainer = Instantiate(tipPrefab, tipsRoot);
            tipContainer.SetTip(tip, currentTip);
            tips.Add(tipContainer);
            buttonTransform.SetAsLastSibling();
            currentTip++;
            
            string nextTip = controller.GetNextTip(currentTip);
            if (nextTip.Equals(""))
            {
                buttonLabel.text = "Подсказки кончились";
                button.interactable = false;
            }
        }

    }
}
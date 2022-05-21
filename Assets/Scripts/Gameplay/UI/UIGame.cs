using System;
using Gameplay.Conrollers;
using Gameplay.Controls;
using Gameplay.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class UIGame : MonoBehaviour
    {
        public UITopBar topBar;
        public UIStartGameButton startGameButton;
        public TMP_Text confirmButtonText;
        public Button confirmButton;
        
        public UISwipe answerPanelSwipe;
        public TouchButton touchButton;
        public UIIntro intro;

        public void EnableStartButton(string gameModeText)
        {
            startGameButton.gameModeText.text = gameModeText;
            startGameButton.gameObject.SetActive(true);
            startGameButton.button.interactable = true;
        }

        public void SetStartInteractable(bool interactable)
        {
            startGameButton.button.interactable = interactable;
        }

        public void DisableStartButton()
        {
            startGameButton.gameObject.SetActive(false);
        }

        public void SetConfirmText(string text, bool interactable = true)
        {
            confirmButtonText.text = text;
            confirmButton.interactable = interactable;
        }
    }
}
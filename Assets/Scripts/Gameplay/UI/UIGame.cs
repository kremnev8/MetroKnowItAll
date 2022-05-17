using Gameplay.Controls;
using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class UIGame : MonoBehaviour
    {
        public UITopBar topBar;
        public UIStartGameButton startGameButton;
        public TMP_Text confirmButtonText;
        public UISwipe answerPanelSwipe;
        public TouchButton touchButton;


        public void EnableStartButton(string gameModeText)
        {
            startGameButton.gameModeText.text = gameModeText;
            startGameButton.gameObject.SetActive(true);
        }

        public void DisableStartButton()
        {
            startGameButton.gameObject.SetActive(false);
        }

        public void SetConfirmText(string text)
        {
            confirmButtonText.text = text;
        }

    }
}
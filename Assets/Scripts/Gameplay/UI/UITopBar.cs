using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class UITopBar : MonoBehaviour
    {
        public TMP_Text gameModeLabel;
        public TMP_Text attemptsLabel;
        public TMP_Text statusLabel;

        private int statusHideTimer;

        private float countdownTimer;

        public void SetCurrentLabel(string text)
        {
            gameModeLabel.text = text;
        }

        public void SetCurrentAttempts(int value)
        {
            attemptsLabel.text = value.ToString();
        }

        public void UpdateStatus(bool lastCorrect, int current, int total)
        {
            string correctText = lastCorrect ? "Верно!" : "Неверно!";
            Color textColor = lastCorrect ? GameController.theme.rightAnswer : GameController.theme.wrongAnswer;
            statusLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{correctText}</color>\nПрогресс {current}/{total}";
            statusLabel.gameObject.SetActive(true);
            statusHideTimer = 120;
        }

        public void StartCountdown(float time)
        {
            countdownTimer = time;
        }

        private void Update()
        {
            if (statusHideTimer > 0)
            {
                statusHideTimer--;
                if (statusHideTimer == 0)
                {
                    statusLabel.gameObject.SetActive(false);
                }
            }

            if (countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;
                if (countdownTimer <= 0)
                {
                    gameModeLabel.text = "";
                    return;
                }
                int minutes = Mathf.FloorToInt(countdownTimer / 60F);
                int seconds = Mathf.FloorToInt(countdownTimer - minutes * 60);

                gameModeLabel.text = $"Перерыв {minutes:0}:{seconds:00}";
            }
        }
    }
}
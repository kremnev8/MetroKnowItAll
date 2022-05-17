using System;
using Gameplay.Conrollers;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    /// <summary>
    /// Top bar UI controller
    /// </summary>
    public class UITopBar : MonoBehaviour
    {
        public TMP_Text gameModeLabel;
        public TMP_Text attemptsLabel;
        public TMP_Text statusLabel;

        public GameObject tipButtonObject;

        public Image attemptsFill;
        public Image troykaImage;
        public float fillSpeed = 1;

        private int statusHideTimer;
        private float targetFillAmount;

        private int fakeAttemptCount;
        private int realAttemptCount;

        private float countdownTimer;

        private void OnEnable()
        {
            SettingsController.difficultyChanged += OnDifficultyChanged;
        }

        private void OnDisable()
        {
            SettingsController.difficultyChanged -= OnDifficultyChanged;
        }
        
        private void OnDifficultyChanged(Difficulty difficulty)
        {
            tipButtonObject.SetActive(difficulty.allowHints);
        }

        public void SetCurrentLabel(string text)
        {
            gameModeLabel.text = text;
        }

        public void SetCurrentAttempts(int current, int newValue, float partial)
        {
            fakeAttemptCount = current;
            realAttemptCount = newValue;
            attemptsLabel.text = current.ToString();
            targetFillAmount = partial;
            attemptsFill.enabled = true;
            troykaImage.enabled = false;
        }
        
        public void SetCurrentAttemptsImmidiate(int value, float partial)
        {
            fakeAttemptCount = value;
            realAttemptCount = value;
            attemptsLabel.text = value.ToString();
            attemptsFill.fillAmount = partial;
            attemptsFill.enabled = true;
            troykaImage.enabled = false;
        }

        public void UpdateStatus(bool lastCorrect, int current, int total)
        {
            string correctText = lastCorrect ? "Верно!" : "Неверно!";
            Color textColor = lastCorrect ? GameController.theme.rightAnswer : GameController.theme.wrongAnswer;
            statusLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{correctText}</color>\nПрогресс {current}/{total}";
            statusLabel.gameObject.SetActive(true);
            statusHideTimer = 120;
        }

        public void UpdateTickets(int tickets)
        {
            attemptsLabel.text = tickets.ToString();
            attemptsFill.enabled = false;
            troykaImage.enabled = true;
        }

        public void ShowMessage(string message)
        {
            statusLabel.text = message;
            statusLabel.gameObject.SetActive(true);
            statusHideTimer = 80;
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

            float fillDiff = Mathf.Abs(targetFillAmount - attemptsFill.fillAmount);
            if (fillDiff > 0.01f)
            {
                if (targetFillAmount > attemptsFill.fillAmount)
                {
                    float target = Mathf.Min(1, targetFillAmount);
                    if (attemptsFill.fillAmount < target)
                    {
                        attemptsFill.fillAmount += fillSpeed * Time.deltaTime;
                    }
                    
                    if (attemptsFill.fillAmount > 0.99f && fakeAttemptCount < realAttemptCount)
                    {
                        attemptsFill.fillAmount = 0;
                        targetFillAmount -= 1;
                        fakeAttemptCount += 1;
                        attemptsLabel.text = fakeAttemptCount.ToString();
                    }
                }
                else
                {
                    attemptsFill.fillAmount -= fillSpeed * Time.deltaTime;
                }
            }
        }
    }
}
using System;
using Gameplay.MetroDisplay.Model;

namespace Gameplay.Conrollers
{
    [Serializable]
    public class Game
    {
        public const string ANY_QUESTIONS = "any";
        
        // Parameters
        public string mode;
        public string questionId;

        //Game state
        public bool isPlaying;
        public int currentGenerator;
        
        public Region currentRegion;

        public int attemptsLeft;
        public int partialAttempts;

        public int correctAnswers;
        public int currentQuestion;
        public float answerTimeElapsed;

        public void Reset()
        {
            isPlaying = false;
            currentGenerator = 0;
            currentRegion = null;
            attemptsLeft = 0;
            partialAttempts = 0;
            correctAnswers = 0;
            currentQuestion = 0;
            answerTimeElapsed = 0;
        }
    }
}
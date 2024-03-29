﻿using Gameplay.Conrollers;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;

namespace Gameplay.Questions
{
    /// <summary>
    /// Base class for all question generators
    /// </summary>
    public abstract class BaseQuestionGenerator
    {
        public MetroRenderer renderer;
        public Metro metro;
        
        public virtual void Init(MetroRenderer _renderer, BaseUIQuestion root)
        {
            renderer = _renderer;
            metro = _renderer.metro;
        }

        public abstract string questionId { get; }

        public abstract void SetRegion(Region region);
        
        public abstract void GenerateNew();
        public abstract string GenerateTip(int tipNumber);
        public abstract bool ValidateAnswer();

        public virtual bool ShouldUse(Game game, int questionNumber)
        {
            if (game.questionId.Equals(Game.ANY_QUESTIONS))
            {
                return true;
            }
            
            return game.questionId.Equals(questionId);
        }

    }
}
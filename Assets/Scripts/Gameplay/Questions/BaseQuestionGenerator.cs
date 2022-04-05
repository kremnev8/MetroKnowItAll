using UnityEngine;

namespace Gameplay.Questions
{
    public abstract class BaseQuestionGenerator
    {
        public MetroRenderer renderer;
        public Metro metro;
        
        public virtual void Init(MetroRenderer _renderer, BaseUIQuestion root)
        {
            renderer = _renderer;
            metro = _renderer.metro;
        }

        public abstract void SetRegion(Region region);
        
        public abstract void GenerateNew();
        public abstract string GenerateTip(int tipNumber);
        public abstract bool ValidateAnswer();
        
    }
}
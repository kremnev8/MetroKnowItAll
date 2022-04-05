using UnityEngine;

namespace Gameplay.Questions
{
    public abstract class BaseUIQuestion : MonoBehaviour
    {
        public new MetroRenderer renderer;
        
        public abstract BaseQuestionGenerator GetController();
        public abstract void HideElements();

    }
}
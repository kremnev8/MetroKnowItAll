using UnityEngine;

namespace Gameplay.Questions
{
    public abstract class BaseUIQuestion : MonoBehaviour
    {
        public abstract BaseQuestionGenerator GetController();
        
    }
}
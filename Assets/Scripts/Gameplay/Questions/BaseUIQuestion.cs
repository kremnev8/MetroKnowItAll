using TMPro;
using UnityEngine;

namespace Gameplay.Questions
{
    public abstract class BaseUIQuestion : MonoBehaviour
    {
        public new MetroRenderer renderer;
        
        public RectTransform bottomPane;
        public TMP_Text questionLabel;
        
        
        public abstract BaseQuestionGenerator GetController();

        public virtual void HideElements()
        {
            questionLabel.text = "";
        }

    }
}
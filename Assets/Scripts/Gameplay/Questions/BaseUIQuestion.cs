using Gameplay.MetroDisplay;
using TMPro;
using UnityEngine;

namespace Gameplay.Questions
{
    /// <summary>
    /// Base class for all question UI classes
    /// </summary>
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
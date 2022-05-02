using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    /// <summary>
    /// Controls version text
    /// </summary>
    public class VersionText : MonoBehaviour
    {
        private TMP_Text label;

        private void Awake()
        {
            label = GetComponent<TMP_Text>();
            label.text = $"Alpha v{Application.version}";
            
        }
    }
}
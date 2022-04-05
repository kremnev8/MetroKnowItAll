using System;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class DynamicText : MonoBehaviour
    {
        public string text;
        public int maxDots;
        
        private TMP_Text label;
        private int counter;
        private int dots;
        
        private void Awake()
        {
            label = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            counter++;
            if (counter > 5)
            {
                counter = 0;
                dots++;
                if (dots > maxDots)
                {
                    dots = 0;
                }
                
                label.text = text +  new string('.', dots);
            }
        }
    }
}
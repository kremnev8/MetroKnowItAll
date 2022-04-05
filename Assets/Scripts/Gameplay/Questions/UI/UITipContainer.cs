﻿using TMPro;
using UnityEngine;

namespace Gameplay.Questions
{
    public class UITipContainer : MonoBehaviour
    {
        public TMP_Text numText;
        public TMP_Text tipText;

        public void SetTip(string text, int index)
        {
            numText.text = (index + 1).ToString();
            tipText.text = text;
        }
    }
}
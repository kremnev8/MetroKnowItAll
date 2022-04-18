using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Gameplay
{
    [Serializable]
    public class Theme
    {
        [HideInInspector]
        public string themeName;
        
        public Color rightAnswer;
        public Color wrongAnswer;
        
        public Color background;
        public Color secondareBackground;
        
        public Color textColor;
        public Color buttonColor;

        public Color lineColor;
        public Color unfocusedColor;

        public Color highlightColor;
    }
    
    [CreateAssetMenu(fileName = "Color Palette", menuName = "SO/New Color Palette", order = 0)]
    public class ColorPalette : ScriptableObject
    {
        public List<Theme> themes = new List<Theme>();
        [SerializeField]
        private bool m_lightTheme;

        public bool lightTheme
        {
            get => m_lightTheme;
            set
            {
                m_lightTheme = value;
                paletteChanged?.Invoke();
            }
        }

        public Theme currentTheme => m_lightTheme ? themes[0] : themes[1];

        public static Action paletteChanged;
#if UNITY_EDITOR
        internal static bool dirty;
        
        private void OnValidate()
        {
            dirty = true;
        }
#endif
    }
}
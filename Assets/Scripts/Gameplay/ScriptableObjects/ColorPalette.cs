using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Defines a theme with all of the palette colors
    /// </summary>
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
    
    /// <summary>
    /// Data store for known <see cref="Theme"/>
    /// </summary>
    [CreateAssetMenu(fileName = "Color Palette", menuName = "SO/New Color Palette", order = 0)]
    public class ColorPalette : ScriptableObject
    {
        public List<Theme> themes = new List<Theme>();
        [SerializeField]
        public int themeIndex;
        
        public bool lightTheme
        {
            get => themeIndex == 0;
            set
            {
                themeIndex = value ? 0 : 1;
                paletteChanged?.Invoke();
            }
        }

        public Theme currentTheme => themes[themeIndex];

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
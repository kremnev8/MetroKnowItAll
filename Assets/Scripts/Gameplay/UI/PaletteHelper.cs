using System;
using Model;
using Platformer.Core;
using UnityEngine;

namespace Gameplay
{
    [ExecuteInEditMode]
    public class PaletteHelper : MonoBehaviour
    {
        public static ColorPalette m_palette;
        public static Theme theme => m_palette.currentTheme;

        public Material lineMaterial;
        public Material crossingMaterial;
        public Material tmpOutlineMaterial;
        
        private static readonly int unfocusedColor = Shader.PropertyToID("_UnfocusedColor");
        private static readonly int backColor = Shader.PropertyToID("_BackColor");
        private static readonly int color = Shader.PropertyToID("_Color");
        private static readonly int underlayColor = Shader.PropertyToID("_UnderlayColor");

        private void Start()
        {
            m_palette = Simulation.GetModel<GameModel>().palette;
            ColorPalette.paletteChanged += UpdateMaterials;
        }

        private void UpdateMaterials()
        {
            lineMaterial.SetColor(backColor, theme.lineColor);
            lineMaterial.SetColor(unfocusedColor, theme.unfocusedColor);
            
            crossingMaterial.SetColor(color, theme.lineColor);
            crossingMaterial.SetColor(unfocusedColor, theme.unfocusedColor);
            
            tmpOutlineMaterial.SetColor(underlayColor, theme.secondareBackground);
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (ColorPalette.dirty)
            {
                ColorPalette.paletteChanged?.Invoke();
                ColorPalette.dirty = false;
            }
        }
#endif
        
    }
}
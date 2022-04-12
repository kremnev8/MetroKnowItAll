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

        private void Start()
        {
            m_palette = Simulation.GetModel<GameModel>().palette;
        }

        private void Update()
        {
            if (ColorPalette.dirty)
            {
                ColorPalette.paletteChanged?.Invoke();
                ColorPalette.dirty = false;
            }
        }
    }
}
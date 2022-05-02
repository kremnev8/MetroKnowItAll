using Gameplay.Core;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay
{
    [ExecuteInEditMode]
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        
        public GameModel model;
        public static Theme theme => instance.model.palette.currentTheme;

        public Material lineMaterial;
        public Material crossingMaterial;
        public Material tmpOutlineMaterial;
        
        private static readonly int unfocusedColor = Shader.PropertyToID("_UnfocusedColor");
        private static readonly int backColor = Shader.PropertyToID("_BackColor");
        private static readonly int color = Shader.PropertyToID("_Color");
        private static readonly int underlayColor = Shader.PropertyToID("_UnderlayColor");
        
        private void Awake()
        {
            instance = this;
            Simulation.SetModel(model);

            ColorPalette.paletteChanged += UpdateMaterials;
            
#if !UNITY_EDITOR
            InputAction backAction = model.input.actions["Back"];
            backAction.started += context =>
            {
                Application.Quit();
            };
#endif
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
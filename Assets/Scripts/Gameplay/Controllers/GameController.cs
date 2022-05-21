using System;
using Gameplay.Controls;
using Gameplay.Core;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = System.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameplay.Conrollers
{
    /// <summary>
    /// Main game controller
    /// </summary>
    [ExecuteInEditMode]
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        public GameModel model;
        public static Theme theme => instance.model.palette.currentTheme;

        public Material lineMaterial;
        public Material crossingMaterial;
        public Material tmpOutlineMaterial;
        public Material highlightMaterial;

        public bool shouldBackCloseGame;

        private InputAction backAction;

        private static readonly int unfocusedColor = Shader.PropertyToID("_UnfocusedColor");
        private static readonly int backColor = Shader.PropertyToID("_BackColor");
        private static readonly int color = Shader.PropertyToID("_Color");
        private static readonly int underlayColor = Shader.PropertyToID("_UnderlayColor");

        private void Awake()
        {
            instance = this;
            Simulation.SetModel(model);

            ColorPalette.paletteChanged += UpdateMaterials;

            backAction = model.input.actions["Back"];
            backAction.performed += OnBack;
        }

        private void OnDestroy()
        {
            backAction.performed -= OnBack;
        }

        private void Start()
        {
            UpdateMaterials();
        }

        private void OnBack(InputAction.CallbackContext obj)
        {
            BackHandler top = BackHandler.GetTop();
            if (top != null)
            {
                top.gameObject.SetActive(false);
            }
            else if (shouldBackCloseGame)
            {
                Exit();
            }
        }

        private static void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
            Application.Quit();
#endif
        }

        private static void ToBackground()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
#endif
        }
        
        private void UpdateMaterials()
        {
            if (lineMaterial != null)
            {
                lineMaterial.SetColor(backColor, theme.lineColor);
                lineMaterial.SetColor(unfocusedColor, theme.unfocusedColor);
            }

            if (crossingMaterial != null)
            {
                crossingMaterial.SetColor(color, theme.lineColor);
                crossingMaterial.SetColor(unfocusedColor, theme.unfocusedColor);
            }

            if (tmpOutlineMaterial != null)
                tmpOutlineMaterial.SetColor(underlayColor, theme.secondareBackground);

            if (highlightMaterial != null)
            {
                highlightMaterial.SetColor(color, theme.background);
            }
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
using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Platformer.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameplay
{
    [ExecuteInEditMode]
    public class PaletteGraphic : MonoBehaviour
    {
        private Graphic graphic;
        private new Renderer renderer;
        
        [NonSerialized]
        internal ColorPalette palette;

        [HideInInspector]
        [SerializeField]
        public string colorName;


        private void Awake()
        {
            graphic = GetComponent<Graphic>();
            renderer = GetComponent<Renderer>();
            
            palette = Simulation.GetModel<GameModel>()?.palette;

            
            ApplyColor();
        }

        public void ApplyColor()
        {
            if (palette == null) palette = Simulation.GetModel<GameModel>()?.palette;
            
            if (palette != null && !String.IsNullOrEmpty(colorName))
            {
                FieldInfo info = typeof(Theme).GetField(colorName);
                Color color = (Color) info.GetValue(palette.currentTheme);

                if (graphic != null)
                {
                    graphic.color = color;
                }else if (renderer != null && renderer.sharedMaterial != null)
                {
                    renderer.sharedMaterial.color = color;
                }
            }
        }

        private void OnDisable()
        {
            ColorPalette.paletteChanged -= ApplyColor;
        }

        private void OnEnable()
        {
            ColorPalette.paletteChanged += ApplyColor;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PaletteGraphic))]
    public class PaletteGraphicEditor : Editor
    {
        SerializedProperty colorNameProperty;
 
        void OnEnable()
        {
            colorNameProperty = serializedObject.FindProperty("colorName");
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PaletteGraphic graphic = (PaletteGraphic) target;

            string[] colorNames = typeof(Theme).GetFields()
                .Where(field => field.FieldType == typeof(Color))
                .Select(field => field.Name).ToArray();

            string currentName = colorNameProperty.stringValue;


            int indexof = Array.IndexOf(colorNames, currentName);

            EditorGUI.BeginChangeCheck();

            indexof = EditorGUILayout.Popup("Palette color", indexof, colorNames);

            if (EditorGUI.EndChangeCheck())
            {
                if (indexof != -1 && !string.IsNullOrEmpty(colorNames[indexof]))
                {
                    colorNameProperty.stringValue = colorNames[indexof];
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (graphic.palette != null)
            {
                Color color = Color.black;
                if (!String.IsNullOrEmpty(graphic.colorName))
                {
                    FieldInfo info = typeof(Theme).GetField(graphic.colorName);
                    color = (Color) info.GetValue(graphic.palette.currentTheme);
                }


                GUI.enabled = false;
                EditorGUILayout.ColorField("Graphic Color", color);
                GUI.enabled = true;
            }

            if (GUILayout.Button("Apply"))
            {
                graphic.ApplyColor();
            }
        }
    }
#endif
}
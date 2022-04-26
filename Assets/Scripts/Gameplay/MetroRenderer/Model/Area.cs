using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.U2D;
using Util;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameplay
{
    [Serializable]
    public class Area
    {
        public static readonly Area Everywhere = new Area(true);
        
        public List<Vector2> points;
        public bool everywhere;
        
        public Area(bool everywhere)
        {
            this.everywhere = everywhere;
            points = new List<Vector2>();
        }
        
        public Area(List<Vector2> points)
        {
            this.points = points;
            everywhere = false;
        }

        public void SetFromSpline(SpriteShapeController shapeController)
        {
            everywhere = false;
            points = new List<Vector2>(shapeController.spline.GetPointCount());
            for (int i = 0; i < shapeController.spline.GetPointCount(); i++)
            {
                points.Add(shapeController.transform.TransformPoint(shapeController.spline.GetPosition(i)));
            }
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Area))]
    public class AreaDrawer : PropertyDrawer
    {
        public static Dictionary<string, SpriteShapeController> s_controllers = new Dictionary<string, SpriteShapeController>();

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Dictionary<string, SpriteShapeController> controllers;
            
            if (property.serializedObject.targetObject is Metro metro)
            {
                controllers = metro.controllers;
            }
            else
            {
                controllers = s_controllers;
            }
            
            EditorGUI.PropertyField(rect, property, label, true);

            if (property.isExpanded)
            {
                float height = EditorGUI.GetPropertyHeight(property, label);

                var fieldRect = new Rect(rect.x, rect.y + height, rect.width, 18);
                var buttonRect = new Rect(rect.x, rect.y + height + 18, rect.width, 18);

                if (controllers.ContainsKey(property.propertyPath))
                {
                    controllers[property.propertyPath] = (SpriteShapeController)EditorGUI.ObjectField(fieldRect, "Shape Controller", controllers[property.propertyPath], typeof(SpriteShapeController), true);
                }
                else
                {
                    controllers[property.propertyPath] = (SpriteShapeController)EditorGUI.ObjectField(fieldRect, "Shape Controller", null, typeof(SpriteShapeController), true);
                }

                
                GUI.enabled = controllers[property.propertyPath] != null;

                if (GUI.Button(buttonRect, "Load from shape"))
                {
                    Area area = (Area)property.GetTargetObjectOfProperty();
                    area.SetFromSpline(controllers[property.propertyPath]);
                }

                GUI.enabled = true;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label) + (property.isExpanded ? 36 : 0);
        }
    }
#endif
}
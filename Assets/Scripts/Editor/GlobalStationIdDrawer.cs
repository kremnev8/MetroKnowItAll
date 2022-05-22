using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gameplay;
using Gameplay.MetroDisplay.Model;
using UnityEditor;
using UnityEngine;
using Util;
using Object = System.Object;

namespace Editor
{
    [CustomPropertyDrawer(typeof(GlobalId))]
    public class GlobalStationIdDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Object target = property.serializedObject.targetObject;

            if (target is Metro)
            {
                return 18;
            }
            
            try
            {
                FieldInfo info = target.GetType().GetFields().First(info => info.FieldType == typeof(Metro));
                if (info.GetValue(target) != null) 
                    return 18;
            }
            catch (Exception e)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            return EditorGUI.GetPropertyHeight(property, label);
        }
        
        private string clickName;
        private int lastlineId;
        private bool clickedLine;
        
        private int lastStationId;
        private bool clickedStation;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            Object target = property.serializedObject.targetObject;

            if (target is Metro metro)
            {
                ShowSelector(rect, property, metro);
            }else
            {
                try
                {
                    FieldInfo info = target.GetType().GetFields().First(info => info.FieldType == typeof(Metro));
                    Metro metro1 = (Metro)info.GetValue(target);
                    ShowSelector(rect, property, metro1);
                }
                catch (Exception e)
                {
                    EditorGUI.PropertyField(rect, property, label, true);
                }
            }

            EditorGUI.EndProperty();
        }

        private void ShowSelector(Rect rect, SerializedProperty property, Metro metro)
        {
            GlobalId currentObject = (GlobalId) property.GetTargetObjectOfProperty();

            float intSize = 35;
            float size = (rect.width - intSize) / 2;
            size = Mathf.Max(size, 50);

            var intRect = new Rect(rect.x, rect.y, intSize, 18);
            var lineRect = new Rect(rect.x + intSize + 5, rect.y, size - 5, 18);
            var stationRect = new Rect(rect.x + intSize + size + 5, rect.y, size - 5, 18);

            int globalId = currentObject.lineId * 100 + currentObject.stationId;


            int value = EditorGUI.IntField(intRect, globalId);

            List<string> lineNames = metro.lines.Select(line => line.currentName).ToList();

            string name = currentObject.lineId < lineNames.Count ? lineNames[currentObject.lineId] : "Select";

            if (EditorGUI.DropdownButton(lineRect, new GUIContent(name), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < lineNames.Count; i++)
                {
                    string item = lineNames[i];
                    int i1 = i;
                    menu.AddItem(new GUIContent(item), i == currentObject.lineId, data =>
                    {
                        lastlineId = i1;
                        clickName = property.propertyPath;
                        clickedLine = true;
                    }, i);
                }

                menu.ShowAsContext();
            }

            if (currentObject.lineId < metro.lines.Count)
            {
                List<string> stationNames =
                    metro.lines[currentObject.lineId].stations.Select(line => line.currentName).ToList();

                name = currentObject.stationId < stationNames.Count ? stationNames[currentObject.stationId] : "Select";

                if (EditorGUI.DropdownButton(stationRect, new GUIContent(name), FocusType.Passive))
                {
                    GenericMenu menu = new GenericMenu();

                    for (int i = 0; i < stationNames.Count; i++)
                    {
                        string item = stationNames[i];
                        int i1 = i;
                        menu.AddItem(new GUIContent(item), i == currentObject.stationId, data =>
                        {
                            lastStationId = i1;
                            clickName = property.propertyPath;
                            clickedStation = true;
                        }, i);
                    }

                    menu.ShowAsContext();
                }
            }
            else
            {
                EditorGUI.LabelField(stationRect, "Select line");
            }

            if (clickedLine && property.propertyPath.Equals(clickName))
            {
                currentObject.lineId = (byte) lastlineId;
                clickedLine = false;
            }
            else if (clickedStation && property.propertyPath.Equals(clickName))
            {
                currentObject.stationId = (byte) lastStationId;
                clickedStation = false;
            }
            else if (value != globalId)
            {
                int lineId = value / 100;
                int stationId = value % 100;
                currentObject.lineId = (byte) lineId;
                currentObject.stationId = (byte) stationId;
            }
        }
    }
}
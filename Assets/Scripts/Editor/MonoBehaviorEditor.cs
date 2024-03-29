﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Util;

namespace Editor
{
    [CanEditMultipleObjects] // Don't ruin everyone's day
    [CustomEditor(typeof(MonoBehaviour), true)] // Target all MonoBehaviours and descendants
    public class MonoBehaviourCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // Draw the normal inspector
            // Get the type descriptor for the MonoBehaviour we are drawing
            var type = target.GetType();

            // Iterate over each private or public instance method (no static methods atm)
            foreach (var method in
                type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                // make sure it is decorated by our custom attribute
                var attributes = method.GetCustomAttributes(typeof(InspectorButtonAttribute), true);
                if (attributes.Length > 0)
                {
                    if (GUILayout.Button("Run: " + method.Name))
                    {
                        MethodInfo methodInfo = target.GetType().GetMethod(method.Name,
                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                        methodInfo?.Invoke(target, Array.Empty<object>());
                    }
                }
            }
        }
    }
}
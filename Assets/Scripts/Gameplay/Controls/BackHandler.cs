using System;
using System.Collections.Generic;
using Gameplay.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Controls
{
    public class BackHandler : MonoBehaviour
    {
        private static List<BackHandler> windowStack = new List<BackHandler>();

        private void OnEnable()
        {
            if (!windowStack.Contains(this))
            {
                windowStack.Add(this);
            }
            else
            {
                windowStack.Remove(this);
                windowStack.Add(this);
            }
        }

        private void OnDisable()
        {
            windowStack.Remove(this);
        }

        public static BackHandler GetTop()
        {
            if (windowStack.Count > 0)
            {
                BackHandler top = windowStack[windowStack.Count - 1];
                windowStack.RemoveAt(windowStack.Count - 1);
                return top;
            }

            return null;
        }
    }
}
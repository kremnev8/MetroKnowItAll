using System;
using System.Collections.Generic;
using Model;
using Platformer.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    [ExecuteInEditMode]
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        
        public GameModel model;

        public static Action onDone;
    
        private void Awake()
        {
            instance = this;
            Simulation.SetModel(model);

#if !UNITY_EDITOR
            InputAction backAction = model.input.actions["Back"];
            backAction.started += context =>
            {
                Application.Quit();
            };
#endif
        }
    }
}
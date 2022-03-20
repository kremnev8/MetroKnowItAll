using System;
using System.Collections.Generic;
using Model;
using Platformer.Core;
using UnityEngine;

namespace Gameplay
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        
        public GameModel model;

        public static Action onDone;
    
        private void Awake()
        {
            instance = this;
            Simulation.SetModel(model);
        }
    }
}
﻿using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "SO/New Player Config", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Zoom")]
        public float zoomSpeed;
        public float minZoom;
        public float maxZoom;
        
        [Header("Movement")]
        public float normalMaxSpeed = 5f;
        public float moveSmoothTime = 1f;
        public float friction = 0.8f;
    }
}
using System;
using UnityEngine;
using Util;

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Defines a connection between two stations. Also defines some display parameters
    /// </summary>
    [Serializable]
    public class MetroConnection : INamedArrayElement
    {
        [HideInInspector]
        public byte lineId;
        
        public byte startStationId;
        public byte endStationId;

        public bool overrideBend;
        public Vector2 bendPoint;
        [Range(0, 8)]
        public float weight = 1;
        
        public int openIn;
        public int closedIn;
        
        public bool IsOpen(int year)
        {
            return year >= openIn && year < closedIn;
        }
        
        public override string ToString()
        {
            return $"J {startStationId} => {endStationId}";
        }

        public string editorName => $"J {startStationId} => {endStationId}";
        public string displayName => "";
    }
}
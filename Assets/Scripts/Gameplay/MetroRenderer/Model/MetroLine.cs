using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using Util;

namespace Gameplay
{
    [Serializable]
    public class MetroLine : INamedArrayElement
    {
        
        public byte lineId;
        public string name;
        
        public LineStyle style;
        public bool useSmoothCurves;
        public Vector2 curveCenter;
        
        public Color lineColor;
        
        [LabeledArray]
        public List<MetroStation> stations = new List<MetroStation>();
        
        [LabeledArray]
        public List<MetroConnection> connections = new List<MetroConnection>();
        public bool simpleLine;

        [Header("Global Line Name Alignment")]
        public bool hideName;
        public NamePosition namePosition;
        public TextAlignmentOptions nameAlignment;
        
        public string displayName => $"{stations.Count}, {name}";
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Statistics;
using TMPro;
using UnityEditor;
using UnityEngine;
using Util;

namespace Gameplay
{
    [Serializable]
    public class MetroLine : INamedArrayElement, IIndexable<int>
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
        
        public string editorName => $"{stations.Count}, {name}";
        public string displayName => name;
        public int index => lineId;
    }
}
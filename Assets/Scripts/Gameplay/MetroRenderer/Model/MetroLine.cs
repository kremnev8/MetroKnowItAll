using System;
using System.Collections.Generic;
using Gameplay.Statistics;
using TMPro;
using UnityEngine;
using Util;

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Defines a line in the metro, contains <see cref="MetroStation"/> and <see cref="MetroConnection"/>
    /// Also has some display parameters
    /// </summary>
    [Serializable]
    public class MetroLine : INamedArrayElement, IIndexable<int>
    {
        
        public byte lineId;
        public string currentName => nameHistory.GetCurrent(MetroRenderer.currentYear);
        public List<TypeDateRange<string>> nameHistory;
        public bool isLooped;
        
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
        public NamePosition namePosition;
        public TextAlignmentOptions nameAlignment;
        
        public int openIn;
        public int closedIn = 3000;
        
        public bool IsOpen(int year)
        {
            return year >= openIn && year < closedIn;
        }
        
        public string editorName => $"{stations.Count}, {currentName}";
        public string displayName => currentName;
        public int index => lineId;
    }
}
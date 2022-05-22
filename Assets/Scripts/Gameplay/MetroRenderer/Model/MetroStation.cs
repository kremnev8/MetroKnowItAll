using System;
using System.Collections.Generic;
using Gameplay.Statistics;
using TMPro;
using UnityEngine;
using Util;

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Defines a station in the metro
    /// Also has some display parameters
    /// </summary>
    [Serializable]
    public class MetroStation : INamedArrayElement, IIndexable<int>
    {
        
        public GlobalId globalId => new GlobalId(lineId, stationId);
        public int index => lineId * 100 + stationId;
        public string currentName => nameHistory.GetCurrentName(MetroRenderer.currentYear);
        
        [HideInInspector]
        public byte lineId;
        public byte stationId;
        public RegionType regionType;
        
        public Vector2 position;
        public NameDateRange[] nameHistory;

        [Header("Override Name Alignment")]
        public bool m_override;
        
        [ShowWhen("m_override")]
        public bool hideName;
        [ShowWhen("m_override")]
        public NamePosition namePosition;
        [ShowWhen("m_override")]
        public TextAlignmentOptions nameAlignment;

        public DateRange[] history;

        public string editorName => $"{(int)globalId} {currentName}";
        public string displayName => currentName;
    }

    [Serializable]
    public struct DateRange
    {
        public int openIn;
        public int closedIn;

        public DateRange(int openIn, int closedIn)
        {
            this.openIn = openIn;
            this.closedIn = closedIn;
        }
    }
}
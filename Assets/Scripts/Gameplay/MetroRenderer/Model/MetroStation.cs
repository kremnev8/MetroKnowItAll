using System;
using TMPro;
using UnityEngine;
using Util;

namespace Gameplay
{
    [Serializable]
    public class MetroStation : INamedArrayElement
    {
        
        public int globalId => lineId * 100 + stationId;
        public string currentName => names[currentNameIndex];
        
        [HideInInspector]
        public byte lineId;
        public byte stationId;
        public RegionType regionType;
        
        public Vector2 position;
        public string[] names;
        public int currentNameIndex;

        [Header("Override Name Alignment")]
        public bool m_override;
        
        [ShowWhen("m_override")]
        public bool hideName;
        [ShowWhen("m_override")]
        public NamePosition namePosition;
        [ShowWhen("m_override")]
        public TextAlignmentOptions nameAlignment;

        public bool isOpen;

        public string editorName => $"{globalId} {currentName}";
        public string displayName => currentName;
    }
}
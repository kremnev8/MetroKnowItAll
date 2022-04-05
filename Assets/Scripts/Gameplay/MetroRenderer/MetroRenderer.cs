using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D;
using Util;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Gameplay
{
    [ExecuteInEditMode]
    public class MetroRenderer : MonoBehaviour
    {
        
#if UNITY_EDITOR        
        public static MetroRenderer instance;
#endif

        [SerializeField] private Transform stationRoot;
        [SerializeField] private Transform lineRoot;
        [SerializeField] private Transform crossingRoot;
        [SerializeField] private Material lineMat;

        [SerializeField] private StationDisplay stationPrefab;
        [SerializeField] private LineDisplay linePrefab;
        [SerializeField] private CrossingDisplay crossingPrefab;    
        
        public Metro metro;

        private Dictionary<int, StationDisplay> stationDisplays = new Dictionary<int, StationDisplay>();
        private List<LineDisplay> lineDisplays = new List<LineDisplay>();
        private List<CrossingDisplay> crossingDisplays = new List<CrossingDisplay>();
        internal bool dirty;

        public int focusedLineId;
        public Area focusArea;
        
        
        public static Vector2 scale = new Vector2(0.1f, -0.1f);
        public static Vector2 translation = new Vector2(-150, 150);
        private static readonly int color = Shader.PropertyToID("_Color");
        private static readonly int focusAreaProp = Shader.PropertyToID("_FocusArea");
        

#if UNITY_EDITOR   
        private void OnEnable()
        {
            instance = this;
        }
#endif

        private void Awake()
        {
            if (Application.isPlaying)
            {
                Regenerate();
            }
        }

        private void Update()
        {
            if (dirty)
            {
                Refresh();
                dirty = false;
            }
        }

        public static Vector2 Transform(Vector2 pos)
        {
            return pos * scale + translation;
        }

        public void Regenerate()
        {
            ClearAll();

            foreach (MetroLine line in metro.lines)
            {
                foreach (MetroStation station in line.stations)
                {
                    Vector3 point = transform.TransformPoint(Transform(station.position) );
                    StationDisplay stationDisplay = Instantiate(stationPrefab, point, Quaternion.identity, stationRoot);

                    stationDisplay.SetStation(station, line);
                    
                    stationDisplays.Add(station.globalId, stationDisplay);
                }

                LineDisplay lineDisplay = Instantiate(linePrefab, lineRoot);
                lineDisplay.SetGroupData(line);
                
                lineDisplays.Add(lineDisplay);
            }

            foreach (MetroCrossing crossing in metro.crossings)
            {
                CrossingDisplay crossingDisplay = Instantiate(crossingPrefab, crossingRoot);
                crossingDisplay.SetCrossing(metro, crossing);
                crossingDisplays.Add(crossingDisplay);
            }
        }

        public void Refresh()
        {
            foreach (StationDisplay display in stationDisplays.Values)
            {
                display.Refresh();
            }

            foreach (LineDisplay display in lineDisplays)
            {
                display.Refresh();
            }
            
            foreach (CrossingDisplay display in crossingDisplays)
            {
                display.Refresh();
            }
        }

        public void ClearAll()
        {
            stationRoot.gameObject.ClearChildren();
            lineRoot.gameObject.ClearChildren();
            crossingRoot.gameObject.ClearChildren();

            stationDisplays.Clear();
            lineDisplays.Clear();
            crossingDisplays.Clear();
        }

        public StationDisplay getStationDisplay(MetroStation station)
        {
            if (stationDisplays.ContainsKey(station.globalId))
            {
                return stationDisplays[station.globalId];
            }

            throw new ArgumentException($"Global ID: {station.globalId} does not exist in display dictionary!");
        }

        public void HideAllLabels()
        {
            foreach (StationDisplay display in stationDisplays.Values)
            {
                display.SetLabelVisible(false, Color.white);
            }
        }

        public void ClearFocus()
        {
            focusArea = Area.Everywhere;
            focusedLineId = -1;
            
            lineMat.SetVector(focusAreaProp, focusArea.GetVector());
            foreach (LineDisplay lineDisplay in lineDisplays)  
            {
                lineDisplay.SetFocused(true);
            }

            foreach (StationDisplay stationDisplay in stationDisplays.Values)
            {
                stationDisplay.SetFocused(true);
            }
        }

        public void FocusLine(int lineId)
        {
            foreach (LineDisplay lineDisplay in lineDisplays)  
            {
                lineDisplay.SetFocused(lineDisplay.line.lineId == lineId);
            }

            foreach (StationDisplay stationDisplay in stationDisplays.Values)
            {
                stationDisplay.SetFocused(stationDisplay.station.lineId == lineId);
            }
            
            focusArea = Area.Everywhere;
            focusedLineId = lineId;
        }

        public void FocusArea(Area section)
        {
            lineMat.SetVector(focusAreaProp, section.GetVector());
            focusArea = section;
            focusedLineId = -1;
        }

        public bool IsFocused(StationDisplay display)
        {
            if (focusedLineId != -1)
            {
                return display.station.lineId == focusedLineId;
            }
            else
            {
                return focusArea.IsInside(Transform(display.station.position));
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MetroRenderer))]
public class MetroEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MetroRenderer renderer = (MetroRenderer) target;
        
        if (GUILayout.Button("Hide Names"))
        {
            renderer.HideAllLabels();
        }
        
        if (GUILayout.Button("Focus"))
        {
            renderer.FocusLine(2);
        }

        if (GUILayout.Button("Regenerate"))
        {
            renderer.Regenerate();
        }
        
        if (GUILayout.Button("Refresh"))
        {
            renderer.Refresh();
        }

        if (GUILayout.Button("Clear"))
        {
            renderer.ClearAll();
        }
    }
}

#endif
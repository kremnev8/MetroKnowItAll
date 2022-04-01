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
        public static MetroRenderer instance;

        public Transform stationRoot;
        public Transform lineRoot;
        public Transform crossingRoot;
        public Material lineMat;

        public Metro metro;
        public static Vector2 scale = new Vector2(0.1f, -0.1f);
        public static Vector2 translation = new Vector2(-150, 150);

        public StationDisplay stationPrefab;
        public LineDisplay linePrefab;
        public CrossingDisplay crossingPrefab;

        private List<StationDisplay> stationDisplays = new List<StationDisplay>();
        private List<LineDisplay> lineDisplays = new List<LineDisplay>();
        private List<CrossingDisplay> crossingDisplays = new List<CrossingDisplay>();
        
        private static readonly int color = Shader.PropertyToID("_Color");

        internal bool dirty;


        private void OnEnable()
        {
            instance = this;
        }

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
                    
                    stationDisplays.Add(stationDisplay);
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
            foreach (StationDisplay display in stationDisplays)
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
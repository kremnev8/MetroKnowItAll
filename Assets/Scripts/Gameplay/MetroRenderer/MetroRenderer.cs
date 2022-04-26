using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using UnityEngine;
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
        [SerializeField] private Transform focusRoot;
        [SerializeField] private Material lineMat;

        [SerializeField] private StationDisplay stationPrefab;
        [SerializeField] private LineDisplay linePrefab;
        [SerializeField] private CrossingDisplay crossingPrefab;
        [SerializeField] private GameObject stationSelectPrefab;
        [SerializeField] private FocusDisplay focusPrefab;

        public Metro metro;

        private Dictionary<int, StationDisplay> stationDisplays = new Dictionary<int, StationDisplay>();
        private List<LineDisplay> lineDisplays = new List<LineDisplay>();
        private List<CrossingDisplay> crossingDisplays = new List<CrossingDisplay>();
        private List<FocusDisplay> focusDisplays = new List<FocusDisplay>();
        internal bool dirty;

        public Region focusRegion;
        
        public bool expectedLabelState = true;

        private GameObject m_stationSelect;
        public GameObject stationSelect
        {
            get
            {
                if (m_stationSelect == null)
                {
                    m_stationSelect = Instantiate(stationSelectPrefab);
                }

                return m_stationSelect;
            }
        }
        
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

        public void Regenerate()
        {
            ClearAll();

            foreach (MetroLine line in metro.lines)
            {
                foreach (MetroStation station in line.stations)
                {
                    Vector3 point = transform.TransformPoint(station.position );
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
            
            foreach (Region region in metro.regions)
            {
                if (region.regionType == RegionType.GLOBAL) continue;
                
                FocusDisplay focusDisplay = Instantiate(focusPrefab, focusRoot);
                focusDisplay.SetRegion(this, region);
                focusDisplays.Add(focusDisplay);
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

            foreach (FocusDisplay display in focusDisplays)
            {
                display.Refresh(false);
            }
        }

        public void ClearAll()
        {
            stationRoot.gameObject.ClearChildren();
            lineRoot.gameObject.ClearChildren();
            crossingRoot.gameObject.ClearChildren();
            focusRoot.gameObject.ClearChildren();

            stationDisplays.Clear();
            lineDisplays.Clear();
            crossingDisplays.Clear();
            focusDisplays.Clear();
        }

        public StationDisplay GetStationDisplay(MetroStation station)
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
                display.SetLabelVisible(false, GameController.theme.textColor);
            }
        }
        
        public void ShowAllLabels()
        {
            try
            {
                foreach (StationDisplay display in stationDisplays.Values)
                {
                    display.SetLabelVisible(true, GameController.theme.textColor);
                }
            }
            catch (Exception e)
            {
               Debug.Log(e.Message);
            }
        }

        public void ClearFocus()
        {
            focusRegion = Region.everywhere;
            
            foreach (LineDisplay lineDisplay in lineDisplays)  
            {
                lineDisplay.SetFocused(true);
            }

            foreach (StationDisplay stationDisplay in stationDisplays.Values)
            {
                stationDisplay.SetFocused(true);
            }
            
            foreach (CrossingDisplay display in crossingDisplays)
            {
                display.SetFocused(true);
            }

            foreach (FocusDisplay display in focusDisplays)
            {
                display.Refresh(false);
            }
        }

        public void FocusRegion(Region region)
        {
            if (region.regionType == RegionType.GLOBAL)
            {
                focusRegion = region;
                foreach (LineDisplay lineDisplay in lineDisplays)
                {
                    lineDisplay.SetFocused(lineDisplay.line.lineId == region.lineId);
                }

                foreach (StationDisplay stationDisplay in stationDisplays.Values)
                {
                    stationDisplay.SetFocused(stationDisplay.station.lineId == region.lineId);
                    stationDisplay.SetInitialVisible(!stationDisplay.station.hideName);
                }

                foreach (CrossingDisplay display in crossingDisplays)
                {
                    display.SetFocused(display.crossing.stationsGlobalIds.Any(id => id.lineId == region.lineId));
                }

                foreach (FocusDisplay display in focusDisplays)
                {
                    display.Refresh(false);
                }
            }
            else
            {
                focusRegion = region;
                
                foreach (LineDisplay lineDisplay in lineDisplays)
                {
                    lineDisplay.SetFocused(true);
                }

                foreach (StationDisplay stationDisplay in stationDisplays.Values)
                {
                    stationDisplay.SetFocused(true);
                    if (!stationDisplay.station.hideName)
                    {
                        stationDisplay.SetInitialVisible(region.Contains(stationDisplay.station));
                    }
                }

                foreach (CrossingDisplay display in crossingDisplays)
                {
                    display.SetFocused(true);
                }
                
                foreach (FocusDisplay display in focusDisplays)
                {
                    display.Refresh(false);
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MetroRenderer))]
public class MetroEditor : Editor
{
    public RegionType regionType;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MetroRenderer renderer = (MetroRenderer) target;

        regionType = (RegionType)EditorGUILayout.EnumPopup("Select region", regionType);
        
        if (GUILayout.Button("Set Focus"))
        {
            renderer.FocusRegion(renderer.metro.regions.First(region => region.regionType == regionType));
        }
        
        if (GUILayout.Button("Toggle Names"))
        {
            if (renderer.expectedLabelState)
            {
                renderer.HideAllLabels();
            }
            else
            {
                renderer.ShowAllLabels();
            }

            renderer.expectedLabelState = !renderer.expectedLabelState;
        }

        if (GUILayout.Button("Clear Focus"))
        {
            renderer.ClearFocus();
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
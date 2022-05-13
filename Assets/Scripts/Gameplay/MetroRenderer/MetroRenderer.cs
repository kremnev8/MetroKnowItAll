using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Conrollers;
using Gameplay.Core;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
using Util;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Gameplay.MetroDisplay
{
    /// <summary>
    /// Controls how entire <see cref="Gameplay.MetroDisplay.Model.Metro"/> is displayed
    /// </summary>
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

        [SerializeField] private HighlightDisplay highlightDisplay;

        public Metro metro;

        private Dictionary<int, StationDisplay> stationDisplays = new Dictionary<int, StationDisplay>();
        private List<LineDisplay> lineDisplays = new List<LineDisplay>();
        private List<CrossingDisplay> crossingDisplays = new List<CrossingDisplay>();
        internal bool dirty;

        public Region focusRegion;
        
        public bool expectedLabelState = true;

        private GameModel model;
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

        private void Start()
        {
            model = Simulation.GetModel<GameModel>();
        }

        private void Update()
        {
            if (dirty)
            {
                Refresh();
                dirty = false;
            }
        }

        /// <summary>
        /// Recreate all display meshes and objects
        /// </summary>
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
        }

        /// <summary>
        /// Refresh renderer state without allocation
        /// </summary>
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
            
            highlightDisplay.Refresh();
        }

        /// <summary>
        /// Destroy all display objects
        /// </summary>
        public void ClearAll()
        {
            stationRoot.gameObject.ClearChildren();
            lineRoot.gameObject.ClearChildren();
            crossingRoot.gameObject.ClearChildren();
            focusRoot.gameObject.ClearChildren();

            stationDisplays.Clear();
            lineDisplays.Clear();
            crossingDisplays.Clear();
            highlightDisplay.Clear();
        }

        /// <summary>
        /// Get Display for a station
        /// </summary>
        /// <returns>Corresponding display</returns>
        /// <exception cref="ArgumentException">thrown if display object does not exist</exception>
        public StationDisplay GetStationDisplay(MetroStation station)
        {
            if (stationDisplays.ContainsKey(station.globalId))
            {
                return stationDisplays[station.globalId];
            }

            throw new ArgumentException($"Global ID: {station.globalId} does not exist in display dictionary!");
        }

        /// <summary>
        /// Stop displaying all station labels
        /// </summary>
        public void HideAllLabels()
        {
            foreach (StationDisplay display in stationDisplays.Values)
            {
                display.SetLabelVisible(false, GameController.theme.textColor);
            }
        }
        
        /// <summary>
        /// Show all station labels
        /// </summary>
        public void ShowAllLabels()
        {
            try
            {
                foreach (StationDisplay display in stationDisplays.Values)
                {
                    Color textColor = GameController.theme.textColor;
                    if (model.statistics.current.unlockedStations.IsUnlocked(display.station))
                    {
                      //  textColor = GameController.theme.rightAnswer;
                    }
                    
                    display.SetLabelVisible(true, textColor);
                }
            }
            catch (Exception e)
            {
               Debug.Log(e.Message);
            }
        }

        /// <summary>
        /// Clear focus and display everything
        /// </summary>
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
                stationDisplay.SetInitialVisible(!stationDisplay.station.hideName);
            }
            
            foreach (CrossingDisplay display in crossingDisplays)
            {
                display.SetFocused(true);
            }

            highlightDisplay.region = focusRegion;
            highlightDisplay.Refresh();
        }

        /// <summary>
        /// Focus a region, other places will be overlayed with white mask, and names will be hidden
        /// </summary>
        /// <param name="region">Target region</param>
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
                    if (!stationDisplay.station.hideName)
                    {
                        stationDisplay.SetInitialVisible(region.Contains(stationDisplay.station));
                    }
                }

                foreach (CrossingDisplay display in crossingDisplays)
                {
                    display.SetFocused(display.crossing.stationsGlobalIds.Any(id => id.lineId == region.lineId));
                }

                highlightDisplay.region = focusRegion;
                highlightDisplay.Refresh();
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
                
                highlightDisplay.region = focusRegion;
                highlightDisplay.Refresh();
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
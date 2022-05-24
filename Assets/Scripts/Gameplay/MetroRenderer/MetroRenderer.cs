using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Conrollers;
using Gameplay.Core;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
using UnityEngine.Serialization;
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

        public static int currentYear
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    if (Simulation.GetModel<GameModel>().renderer == null)
                    {
                        return 2022;
                    }
                    return Simulation.GetModel<GameModel>().renderer.m_year;
                }

                return instance != null ? instance.m_year : 2999;
#else
                if (Simulation.GetModel<GameModel>().renderer == null)
                {
                    return 2022;
                }
                return Simulation.GetModel<GameModel>().renderer.m_year;
#endif
            }
            set
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Simulation.GetModel<GameModel>().renderer.m_year = value;
                }
                else if (instance != null)
                {
                    instance.m_year = value;
                }
#else
#endif
            }
        }


        [SerializeField] private Transform stationRoot;
        [SerializeField] private Transform lineRoot;
        [SerializeField] private Transform crossingRoot;
        [SerializeField] private Material lineMat;

        [SerializeField] private StationDisplay stationPrefab;
        [SerializeField] private LineDisplay linePrefab;
        [SerializeField] private CrossingDisplay crossingPrefab;
        [SerializeField] private GameObject stationSelectPrefab;

        [SerializeField] private HighlightDisplay highlightDisplay;

        public Metro metro;

        [FormerlySerializedAs("year")] [Range(1935, 2022)] [SerializeField]
        private int m_year;

        public int year
        {
            get => m_year;
            set
            {
                m_year = value;
                Regenerate();
            }
        }

        private Dictionary<int, StationDisplay> stationDisplays = new Dictionary<int, StationDisplay>();
        private List<LineDisplay> lineDisplays = new List<LineDisplay>();
        private List<CrossingDisplay> crossingDisplays = new List<CrossingDisplay>();

#if UNITY_EDITOR
        public bool dirty;
        public bool veryDirty;
#endif

        public bool trueView;

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
            bool isPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(this);
            if (isPrefabInstance)
            {
                veryDirty = true;
            }
        }
#endif

        private void Start()
        {
            model = Simulation.GetModel<GameModel>();
            if (Application.isPlaying)
            {
                Regenerate();
            }
        }

        public void SetMetro(Metro newMetro)
        {
            metro = newMetro;
        }

#if UNITY_EDITOR
        private void Update()
        {
            bool isPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(this);
            if (isPrefabInstance)
            {
                if (veryDirty)
                {
                    Regenerate();
                    veryDirty = false;
                }
                else if (dirty)
                {
                    Refresh();
                    dirty = false;
                }
            }
        }
#endif

        /// <summary>
        /// Recreate all display meshes and objects
        /// </summary>
        public void Regenerate()
        {
            ClearAll();

            foreach (MetroLine line in metro.lines)
            {
                if (line.IsOpen(m_year))
                {
                    foreach (MetroStation station in line.stations)
                    {
                        if (station.isOpen || !trueView)
                        {
                            Vector3 point = transform.TransformPoint(station.position);
                            StationDisplay stationDisplay = Instantiate(stationPrefab, point, Quaternion.identity, stationRoot);

                            stationDisplay.SetStation(this, station, line);

                            stationDisplays.Add(station.globalId, stationDisplay);
                        }
                    }

                    LineDisplay lineDisplay = Instantiate(linePrefab, lineRoot);
                    lineDisplay.SetGroupData(line);

                    lineDisplays.Add(lineDisplay);
                }
            }

            foreach (MetroCrossing crossing in metro.crossings)
            {
                if (crossing.IsOpen(m_year))
                {
                    CrossingDisplay crossingDisplay = Instantiate(crossingPrefab, crossingRoot);
                    crossingDisplay.SetCrossing(metro, crossing);
                    crossingDisplays.Add(crossingDisplay);
                }
            }

            highlightDisplay.Init(metro);
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

            highlightDisplay.Init(metro);
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
        /// Get Display for a station
        /// </summary>
        /// <returns>Corresponding display</returns>
        /// <exception cref="ArgumentException">thrown if display object does not exist</exception>
        public StationDisplay GetStationDisplay(GlobalId globalId)
        {
            if (stationDisplays.ContainsKey(globalId))
            {
                return stationDisplays[globalId];
            }

            throw new ArgumentException($"Global ID: {globalId} does not exist in display dictionary!");
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
                stationDisplay.SetInitialVisible(IsDisplayPrimary(stationDisplay));
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
            if (region.regionType == RegionType.GLOBAL_LINE && region.lineId != -1)
            {
                focusRegion = region;
                foreach (LineDisplay lineDisplay in lineDisplays)
                {
                    lineDisplay.SetFocused(lineDisplay.line.lineId == region.lineId);
                }

                foreach (StationDisplay stationDisplay in stationDisplays.Values)
                {
                    stationDisplay.SetFocused(stationDisplay.station.lineId == region.lineId);
                    if (IsDisplayPrimary(stationDisplay))
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
                    if (IsDisplayPrimary(stationDisplay))
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

        public bool IsDisplayPrimary(StationDisplay display)
        {
            try
            {
                GlobalId id = GetPrimaryDisplayId(display);

                return display.station.globalId == id;
            }
            catch (InvalidOperationException e)
            {
            }

            return true;
        }

        public StationDisplay GetPrimaryDisplay(StationDisplay current)
        {
            try
            {
                GlobalId id = GetPrimaryDisplayId(current);

                if (id == current.station.globalId)
                {
                    return current;
                }

                return GetStationDisplay(id);
            }
            catch (Exception e)
            {
                return current;
            }
        }
        
        private GlobalId GetPrimaryDisplayId(StationDisplay display)
        {
            IEnumerable<MetroCrossing> crossings = metro.crossings.Where(metroCrossing => metroCrossing.stationsGlobalIds.Contains(display.station.globalId));
            GlobalId id = crossings
                .SelectMany(crossing => crossing.stationsGlobalIds)
                .Select(globalId => metro.GetStation(globalId))
                .OrderByDescending(station => station.namePriority)
                .First(station =>
                {
                    return station.isOpen &&
                           station.currentName.Equals(display.station.currentName);
                }).globalId;
            return id;
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

        MetroRenderer renderer = (MetroRenderer)target;

        regionType = (RegionType)EditorGUILayout.EnumPopup("Select region", regionType);

        if (GUILayout.Button("Set Focus"))
        {
            renderer.FocusRegion(new Region(regionType, -1));
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
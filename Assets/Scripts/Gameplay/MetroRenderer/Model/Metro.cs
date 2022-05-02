﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Util;
using System.IO;
using UnityEngine.U2D;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using UnityEditor;

#endif

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Main metro data class, contains all information about the metro
    /// </summary>
    [CreateAssetMenu(fileName = "Metro", menuName = "SO/New Metro", order = 0)]
    public class Metro : ScriptableObject
    {
        [LabeledArray] public List<MetroLine> lines = new List<MetroLine>();
        public List<MetroCrossing> crossings = new List<MetroCrossing>();
        public List<Region> regions = new List<Region>();

#if UNITY_EDITOR
        public Dictionary<string, SpriteShapeController> controllers = new Dictionary<string, SpriteShapeController>();
#endif
        
        /// <summary>
        /// Get all stations with a name
        /// </summary>
        /// <param name="name">Needed name</param>
        /// <returns>All matching stations</returns>
        public List<MetroStation> GetStationsByName(string name)
        {
            List<MetroStation> stations = new List<MetroStation>();
            foreach (MetroLine line in lines)
            {
                try
                {
                    stations.Add(line.stations.First(station =>
                    {
                        return station.currentName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0;
                    }));
                }
                catch (InvalidOperationException e)
                {
                }
            }

            return stations;
        }
        
        /// <summary>
        /// Get stations for line and station id
        /// </summary>
        public MetroStation GetStation(int lineId, int stationId)
        {
            return lines[lineId].stations[stationId];
        }
        
        /// <summary>
        /// Pick a stations that is near another station
        /// </summary>
        public MetroStation PickStationNear(MetroStation station)
        {
            int startIndex = Mathf.Max(0, station.stationId - 2);
            int endIndex = Mathf.Min(lines[station.lineId].stations.Count, station.stationId + 2);
            int tipStation = station.stationId.ConstrainedRandom(startIndex, endIndex);

            return lines[station.lineId].stations[tipStation];
        }

        /// <summary>
        /// Pick a random station within the region
        /// </summary>
        public MetroStation PickRandomStation(Region region)
        {
            if (region.regionType == RegionType.GLOBAL)
            {
                List<MetroStation> stations = lines[region.lineId].stations;
                int index = Random.Range(0, stations.Count);
                return stations[index];
            }
            else
            {
                List<MetroStation> regionStations = new List<MetroStation>();
                foreach (MetroLine line in lines)
                {
                    regionStations.AddRange(
                        line.stations
                            .Where(region.Contains));
                    
                }
                int index = Random.Range(0, regionStations.Count);
                return regionStations[index];
            }
        }
        
        /// <summary>
        /// Pick a random station within the region
        /// Exclude all stations that are in the blacklist
        /// </summary>
        /// <param name="blacklist">List of global station id's to ignore</param>
        public MetroStation PickRandomStation(Region region, List<int> blacklist)
        {
            if (region.regionType == RegionType.GLOBAL)
            {
                List<MetroStation> stations = lines[region.lineId].stations;

                int index = RandomUtils.ConstrainedRandom(stationId => !blacklist.Contains(stations[stationId].globalId), 0, stations.Count);

                return stations[index];
            }
            else
            {
                List<MetroStation> regionStations = new List<MetroStation>();
                foreach (MetroLine line in lines)
                {
                    regionStations.AddRange(
                        line.stations
                            .Where(region.Contains));
                    
                }
                int index = RandomUtils.ConstrainedRandom(stationId => !blacklist.Contains(regionStations[stationId].globalId), 0, regionStations.Count);
                return regionStations[index];
            }
        }
        
        /// <summary>
        /// Pick a line of stations in a random place within region
        /// Exclude all stations that are in the blacklist
        /// </summary>
        /// <param name="size">How many stations range has to have</param>
        /// <param name="blacklist">List of global station id's to ignore</param>
        public List<MetroStation> PickRandomStationRange(Region region, int size, List<int> blacklist)
        {
            if (region.lineId != -1)
            {
                List<MetroStation> stations = lines[region.lineId].stations;

                int index = RandomUtils.ConstrainedRandom(stationId =>
                {
                    if (stationId + size - 1 < stations.Count)
                    {
                        for (int i = stationId; i < stationId + size; i++)
                        {
                            if (blacklist.Contains(stations[i].globalId) || !region.Contains(stations[i]))
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                    return false;
                }, 0, stations.Count);

                return stations.GetRange(index, size);
            }
            else
            {
               List<MetroLine> filteredLines = lines.Where(line => line.stations.Count(station =>
               {
                   return !blacklist.Contains(station.globalId) && region.Contains(station);
               }) >= size).ToList();
               int index = Random.Range(0, filteredLines.Count);

               return PickRandomStationRange(new Region(region.regionType, region.area, filteredLines[index].lineId), size, blacklist);
            }
        }

        /// <summary>
        /// Pick random line within region
        /// </summary>
        public MetroLine PickRandomLine(Region region)
        {
            List<MetroLine> filteredLines = lines.Where(line => line.stations.Count(region.Contains) >= 1).ToList();
            int index = Random.Range(0, filteredLines.Count);
            
            return filteredLines[index];
        }

        private void OnValidate()
        {
            Debug.Log("Validate");
            foreach (MetroLine line in lines)
            {
                for (int i = 0; i < line.stations.Count; i++)
                {
                    MetroStation station = line.stations[i];
                    station.lineId = line.lineId;
                    station.stationId = (byte) i;
                }

                if (line.simpleLine && line.stations.Count > 0)
                {
                    if (line.connections.Capacity < line.stations.Count - 1)
                    {
                        line.connections.Capacity = line.stations.Count - 1;
                    }

                    for (int i = 0; i < line.stations.Count - 1; i++)
                    {
                        if (i < line.connections.Count)
                        {
                            line.connections[i].startStationId = (byte) i;
                            line.connections[i].endStationId = (byte) (i + 1);
                            line.connections[i].lineId = line.lineId;
                        }
                        else
                        {
                            line.connections.Add(new MetroConnection()
                            {
                                startStationId = (byte) i,
                                endStationId = (byte) (i + 1),
                                isOpen = true,
                                lineId = line.lineId
                            });
                        }
                    }
                }
                else
                {
                    foreach (MetroConnection connection in line.connections)
                    {
                        connection.lineId = line.lineId;
                    }
                }
            }

#if UNITY_EDITOR
            if (MetroRenderer.instance != null && MetroRenderer.instance.metro == this)
            {
                MetroRenderer.instance.dirty = true;
            }
#endif
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Metro))]
    public class MetroEditor : Editor
    {
        
        public static Vector2 scale = new Vector2(0.1f, -0.1f);
        public static Vector2 translation = new Vector2(-150, 150);
        
        public static Vector2 Transform(Vector2 pos)
        {
            return pos * scale + translation;
        }
        
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Import Region Data"))
            {
                Metro metro = (Metro) target;
                string path = EditorUtility.OpenFilePanel("Load region data", "", "json");
                string json = File.ReadAllText(path, Encoding.UTF8);
                JObject obj = JObject.Parse(json);
                foreach (int index in (int[])Enum.GetValues(typeof(RegionType)))
                {
                    RegionType type = (RegionType)index;
                    if (type == RegionType.GLOBAL) continue;
                    
                    string key =  Region.GetFileName(type);
                    if (!obj.ContainsKey(key)) continue;
                    
                    JToken stations = obj[key];
                    foreach (string stationName in stations)
                    {
                        try
                        {
                            List<MetroStation> stationsL = metro.GetStationsByName(stationName);
                            foreach (MetroStation station in stationsL)
                            {
                                station.regionType = type;
                            }
                        }
                        catch (InvalidOperationException e)
                        {
                            Debug.LogError($"{e.Message}, stacktrace:\n{e.StackTrace}");
                        }
                    }
                }
                Debug.Log("Success!");
            }

            if (GUILayout.Button("Import"))
            {
                Metro metro = (Metro) target;
                string path = EditorUtility.OpenFilePanel("Load metro data", "", "json");
                string json = File.ReadAllText(path, Encoding.UTF8);
                JsonUtility.FromJsonOverwrite(json, metro);
            }

            if (GUILayout.Button("Export"))
            {
                Metro metro = (Metro) target;
                string path = EditorUtility.SaveFilePanel("Save metro data", "", "metro.json", "json");
                string json = JsonUtility.ToJson(metro, true);
                File.WriteAllText(path, json, Encoding.UTF8);
                Debug.Log("Exported successfully!");
            }
        }
    }
#endif
}
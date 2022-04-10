﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Util;
using System.IO;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Gameplay
{
    [CreateAssetMenu(fileName = "Metro", menuName = "SO/New Metro", order = 0)]
    public class Metro : ScriptableObject
    {
        [LabeledArray] public List<MetroLine> lines = new List<MetroLine>();
        public List<MetroCrossing> crossings = new List<MetroCrossing>();

        public MetroStation GetStation(int lineId, int stationId)
        {
            return lines[lineId].stations[stationId];
        }
        
        public MetroStation PickStationNear(MetroStation station)
        {
            int startIndex = Mathf.Max(0, station.stationId - 2);
            int endIndex = Mathf.Min(lines[station.lineId].stations.Count, station.stationId + 2);
            int tipStation = station.stationId.ConstrainedRandom(startIndex, endIndex);

            return lines[station.lineId].stations[tipStation];
        }

        public MetroStation PickRandomStation(int lineId)
        {
            List<MetroStation> stations = lines[lineId].stations;
            int index = Random.Range(0, stations.Count);
            return stations[index];
        }
        
        public MetroStation PickRandomStation(int lineId, List<int> blacklist)
        {
            List<MetroStation> stations = lines[lineId].stations;

            int index = Extension.ConstrainedRandom(stationId =>
            {
                return !blacklist.Contains(stations[stationId].globalId);
            }, 0, stations.Count);
                
            return stations[index];
        }
        
        public List<MetroStation> PickRandomStationRange(int lineId, int size, List<int> blacklist)
        {
            List<MetroStation> stations = lines[lineId].stations;

            int index = Extension.ConstrainedRandom(stationId =>
            {
                if (stationId + size - 1 < stations.Count)
                {
                    for (int i = stationId; i < stationId + size; i++)
                    {
                        if (blacklist.Contains(stations[i].globalId))
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

        public MetroLine PickRandomLine()
        {
            int index = Extension.ConstrainedRandom(
                lineId =>
                {
                    return lines[lineId].stations.Count <= 0;
                }, 0, lines.Count);
            
            
            return lines[index];
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
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

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
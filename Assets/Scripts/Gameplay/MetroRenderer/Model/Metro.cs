using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Util;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gameplay
{
    [CreateAssetMenu(fileName = "Metro", menuName = "SO/New Metro", order = 0)]
    public class Metro : ScriptableObject
    {
        [LabeledArray]
        public List<MetroLine> lines = new List<MetroLine>();
        public List<MetroCrossing> crossings = new List<MetroCrossing>();

        private void OnValidate()
        {
            Debug.Log("Validate");
            foreach (MetroLine line in lines)
            {
                for (int i = 0; i < line.stations.Count; i++)
                {
                    MetroStation station = line.stations[i];
                    station.lineId = line.lineId;
                    station.stationId = (byte)i;
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
                                startStationId = (byte)i,
                                endStationId = (byte)(i + 1),
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

            if (MetroRenderer.instance != null && MetroRenderer.instance.metro == this)
            {
                MetroRenderer.instance.dirty = true;
            }
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
                string path = EditorUtility.OpenFilePanel("Load metro data", "","json");
                string json = File.ReadAllText(path, Encoding.UTF8);
                JsonUtility.FromJsonOverwrite(json, metro);
            }
            
            if (GUILayout.Button("Export"))
            {
                Metro metro = (Metro) target;
                string path = EditorUtility.SaveFilePanel("Save metro data", "", "metro.json","json");
                string json = JsonUtility.ToJson(metro, true);
                File.WriteAllText(path, json, Encoding.UTF8);
                Debug.Log("Exported successfully!");
            }
        }
    }
#endif
}


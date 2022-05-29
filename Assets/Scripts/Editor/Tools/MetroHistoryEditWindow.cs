using System.Collections.Generic;
using System.Linq;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;
using UnityEditor;
using UnityEngine;
using Util;

namespace Editor.Tools
{

    /// <summary>
    /// Utility window to help modify <see cref="Metro"/> objects using intuitive interface.
    /// </summary>
    public class MetroHistoryEditWindow : EditorWindow
    {
        [MenuItem("Tools/Metro History Editor", false)]
        public static void DoWindow()
        {
            var window = GetWindowWithRect<MetroHistoryEditWindow>(new Rect(0, 0, 300, 170));
            window.SetTargetObject(Selection.activeGameObject);
            window.Show();
        }

        private GameObject target;
        private StationDisplay targetDisplay;
        private MetroStation station;

        private Metro metro;

        private string currentName;
        private bool open;
        private Vector2 position;
        private int currentYear;

        private int addConnStationId;

        private int addlineId;
        private Vector2 addPosition;
        public string addName;

        public MetroHistoryEditWindow()
        {
            titleContent.text = "Metro History Editor";
        }

        public void OnSelectionChange()
        {
            SetTargetObject(Selection.activeGameObject);
        }

        public void SetTargetObject(GameObject newTarget)
        {
            if (newTarget == null)
            {
                target = null;
                targetDisplay = null;
                Repaint();
                return;
            }

            StationDisplay display = newTarget.GetComponent<StationDisplay>();
            if (display == null)
            {
                target = null;
                targetDisplay = null;
                Repaint();
                return;
            }

            target = newTarget;
            targetDisplay = display;

            RefreshData();

            if (MetroRenderer.instance != null)
            {
                metro = MetroRenderer.instance.metro;
            }

            Repaint();
        }

        private void RefreshData()
        {
            currentYear = MetroRenderer.currentYear;

            if (targetDisplay != null)
            {
                station = targetDisplay.station;
                currentName = station.currentName;
                open = station.isOpen;
            }
        }

        public void OnGUI()
        {
            if (metro == null || MetroRenderer.instance == null)
            {
                EditorGUILayout.HelpBox("Не найден объект метро! Пожайлуста выберите метро в MetroRenderer или перезагрузите сцену!", MessageType.Info);
                return;
            }


            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Ренерер");

            EditorGUI.BeginChangeCheck();

            currentYear = EditorGUILayout.IntSlider("Год", currentYear, 1935, 2022);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("- 1"))
            {
                currentYear -= 1;
                currentYear = Mathf.Clamp(currentYear, 1935, 2022);
                MetroRenderer.currentYear = currentYear;
                MetroRenderer.instance.Regenerate();
                RefreshData();
            }

            if (GUILayout.Button("+ 1"))
            {
                currentYear += 1;
                currentYear = Mathf.Clamp(currentYear, 1935, 2022);
                MetroRenderer.currentYear = currentYear;
                MetroRenderer.instance.Regenerate();
                RefreshData();
            }

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                MetroRenderer.currentYear = currentYear;
                MetroRenderer.instance.dirty = true;
                RefreshData();
            }

            if (GUILayout.Button("Обновить!"))
            {
                MetroRenderer.instance.Refresh();
            }

            if (GUILayout.Button("Регенерировать!"))
            {
                MetroRenderer.instance.Regenerate();
            }

            GUILayout.Space(30);
            EditorGUILayout.LabelField("Информация о станций");

            if (target == null)
            {
                EditorGUILayout.HelpBox("Выберите объект с StationDisplay!", MessageType.Info, true);
            }
            else
            {
                #region Stations

                EditorGUILayout.LabelField($"Станция: {station.currentName}");
                EditorGUILayout.LabelField($"Линия {metro.lines[station.lineId].currentName}");

                Dictionary<int, string> info = new Dictionary<int, string>();

                foreach (TypeDateRange<bool> entry in station.history)
                {
                    if (entry.openIn != 0 && !info.ContainsKey(entry.openIn))
                    {
                        info.Add(entry.openIn, entry.value ? "Открыта" : "Закрыта");
                    }
                }

                foreach (TypeDateRange<string> entry in station.nameHistory)
                {
                    if (entry.openIn != 0)
                    {
                        if (info.ContainsKey(entry.openIn))
                        {
                            info[entry.openIn] += " " + entry.value;
                        }
                        else
                        {
                            info.Add(entry.openIn, $"Переименована в {entry.value}");
                        }
                    }
                }

                List<int> keys = info.Keys.ToList();
                keys.Sort();

                foreach (int key in keys)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField($"{key}: {info[key]}");

                    if (info[key].Contains("Переименована"))
                    {
                        if (GUILayout.Button("Удалить"))
                        {
                            station.nameHistory.Remove(key);
                            EditorUtility.SetDirty(metro);
                            RefreshData();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Удалить"))
                        {
                            station.history.Remove(key);
                            EditorUtility.SetDirty(metro);
                            RefreshData();
                        }
                    }


                    EditorGUILayout.EndHorizontal();
                }

                currentName = EditorGUILayout.TextField("Название", currentName);
                open = EditorGUILayout.Toggle("Открыта", open);
                station.namePriority = (byte)EditorGUILayout.IntField("Приоритет", station.namePriority);

                if (GUILayout.Button("Применить!"))
                {
                    if (currentName != station.currentName)
                    {
                        station.nameHistory.Insert(currentYear, currentName);
                        EditorUtility.SetDirty(metro);
                        RefreshData();
                    }

                    if (open != station.isOpen)
                    {
                        station.history.Insert(currentYear, open);
                        EditorUtility.SetDirty(metro);
                        RefreshData();
                    }

                    Vector2 disPos = targetDisplay.transform.position;
                    if ((disPos - station.position).magnitude > 0.1f)
                    {
                        station.position = disPos;
                        EditorUtility.SetDirty(metro);
                        RefreshData();
                    }
                }

                GUILayout.Space(30);
                if (GUILayout.Button("Открыть всю ветку"))
                {
                    foreach (MetroStation stations1 in metro.lines[station.lineId].stations)
                    {
                        stations1.history.Insert(currentYear, true);
                    }   
                }

                #endregion

                GUILayout.Space(30);
                EditorGUILayout.LabelField("Информация о перегонах");

                #region Connections

                MetroLine line = metro.lines[station.lineId];
                List<MetroStation> stations = line.stations;

                IEnumerable<MetroConnection> connections = line.connections
                    .Where(connection => connection.startStationId == station.stationId || connection.endStationId == station.stationId);

                foreach (MetroConnection connection in connections)
                {
                    EditorGUILayout.LabelField($"Перегон {stations[connection.startStationId].currentName} => {stations[connection.endStationId].currentName}");

                    connection.overrideBend = EditorGUILayout.Toggle("Изменить угол", connection.overrideBend);
                    connection.bendPoint = EditorGUILayout.Vector2Field("Позиция угла", connection.bendPoint);
                    
                    connection.openIn = EditorGUILayout.IntField("Открыт С", connection.openIn);
                    connection.closedIn = EditorGUILayout.IntField("По", connection.closedIn);

                    if (GUILayout.Button("Применить!"))
                    {
                        EditorUtility.SetDirty(metro);
                        RefreshData();
                    }

                    if (GUILayout.Button("Сменить направление!"))
                    {
                        (connection.startStationId, connection.endStationId) = (connection.endStationId, connection.startStationId);
                        EditorUtility.SetDirty(metro);
                        RefreshData();
                    }
                        
                }
                
                GUILayout.Space(30);
                EditorGUILayout.LabelField("Добавить перегон");
                
                addConnStationId = EditorGUILayout.Popup("Станция", addConnStationId, stations.Select(metroStation => metroStation.currentName).ToArray());
                if (addConnStationId >= 0 && addConnStationId < stations.Count)
                {
                    if (GUILayout.Button("Добавить!"))
                    {
                        MetroConnection connection = new MetroConnection()
                        {
                            lineId = station.lineId,
                            startStationId = station.stationId,
                            endStationId = (byte)addConnStationId,
                            openIn = 0,
                            closedIn = 3000
                        };
                        line.connections.Add(connection);
                        EditorUtility.SetDirty(metro);
                        RefreshData();
                    }
                }

                #endregion
            }

            GUILayout.Space(30);
            EditorGUILayout.LabelField("Добавить станцию");

            addlineId = EditorGUILayout.Popup("Линия", addlineId, metro.lines.Select(line => line.currentName).ToArray());
            if (addlineId >= 0 && addlineId < metro.lines.Count)
            {
                addPosition = EditorGUILayout.Vector2Field("Позиция", addPosition);
                addName = EditorGUILayout.TextField("Название", addName);

                if (GUILayout.Button("Добавить!"))
                {
                    MetroStation newStation = new MetroStation
                    {
                        position = addPosition,
                        nameHistory = new List<TypeDateRange<string>>
                        {
                            new TypeDateRange<string>(addName, 0, 3000)
                        },
                        history = new List<TypeDateRange<bool>>
                        {
                            new TypeDateRange<bool>(true, 0, 3000)
                        },
                        lineId = (byte)addlineId
                    };
                    metro.lines[addlineId].stations.Add(newStation);
                    EditorUtility.SetDirty(metro);
                    RefreshData();
                }
            }

            EditorGUILayout.EndVertical();
        }


        // Window has been selected
        void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            // Do your drawing here using Handles.
            
            if (target != null)
            {
                MetroLine line = metro.lines[station.lineId];

                IEnumerable<MetroConnection> connections = line.connections
                    .Where(connection => connection.startStationId == station.stationId || connection.endStationId == station.stationId);

                foreach (MetroConnection connection in connections)
                {
                    if (connection.overrideBend)
                    {
                        connection.bendPoint = Handles.PositionHandle(connection.bendPoint, Quaternion.identity);
                    }
                }
            }
            
            Handles.BeginGUI();

            // Do your drawing here using GUI.
            Handles.EndGUI();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
using Util;

namespace Gameplay.MetroDisplay
{
    /// <summary>
    /// Controls how <see cref="Gameplay.MetroDisplay.Model.MetroLine"/> are displayed
    /// </summary>
    public class LineDisplay : MonoBehaviour
    {
        public List<LineSubDisplay> subDisplays;
        public MetroLine line;

        public LineSubDisplay subDisplayPrefab;

        public void SetFocused(bool value)
        {
            foreach (LineSubDisplay display in subDisplays)
            {
                display.SetFocusedInternal(value);
            }
        }
        
        public void Refresh()
        {
            bool isOpen = line.IsOpen(MetroRenderer.currentYear);
            gameObject.SetActive(isOpen);
            
            foreach (LineSubDisplay subDisplay in subDisplays)
            {
                subDisplay.Refresh();
            }
        }
        
        public void SetGroupData(MetroLine _line)
        {
            line = _line;
            List<List<ConnData>> groups = GetSortedLine();

            foreach (List<ConnData> points in groups)
            {
                LineSubDisplay lineObject = Instantiate(subDisplayPrefab, transform);
                lineObject.SetGroupData(points, this);
                
                subDisplays.Add(lineObject);
            }
        }
        
        private List<List<ConnData>> GetSortedLine()
        {
            List<List<ConnData>> allLines = new List<List<ConnData>>();

            if (line.connections.Count == 0) return allLines;

            List<MetroConnection> connections = new List<MetroConnection>(
                line.connections
                    .Where(connection =>
                    {
                        return line.stations[connection.startStationId].isOpen &&
                               line.stations[connection.endStationId].isOpen &&
                                connection.IsOpen(MetroRenderer.currentYear);
                    }));

            while (connections.Count > 0)
            {
                MetroConnection startConn = connections[0];
                connections.RemoveAt(0);

                Vector2 currentStart = line.stations[startConn.startStationId].position;
                Vector2 currentEnd = line.stations[startConn.endStationId].position;


                List<ConnData> points = new List<ConnData>
                {
                    new ConnData(currentStart, startConn),
                    new ConnData(currentEnd, startConn)
                };

                bool foundMatch = true;

                while (foundMatch && connections.Count > 0)
                {
                    foundMatch = false;
                    for (int i = 0; i < connections.Count; i++)
                    {
                        MetroConnection connection = connections[i];
                        Vector2 start = line.stations[connection.startStationId].position;
                        Vector2 end = line.stations[connection.endStationId].position;

                        if (start.Equals(currentEnd))
                        {
                            points.Add(new ConnData(end, connection));
                            currentEnd = end;
                            connections.RemoveAt(i);
                            foundMatch = true;
                        }
                        else if (end.Equals(currentStart))
                        {
                            points.Insert(0, new ConnData(start, connection));
                            currentStart = start;
                            connections.RemoveAt(i);
                            foundMatch = true;
                        }
                    }
                }

                allLines.Add(points);
            }

            return allLines;
        }
    }
}
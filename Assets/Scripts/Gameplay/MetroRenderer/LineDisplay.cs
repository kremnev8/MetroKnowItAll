using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Util;

namespace Gameplay
{
    public class LineDisplay : MonoBehaviour
    {
        public List<LineSubDisplay> subDisplays;
        public MetroLine line;

        public LineSubDisplay subDisplayPrefab;

        public void Refresh()
        {
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
                lineObject.SetGroupData(points, line);
                
                subDisplays.Add(lineObject);
            }
        }
        
        public List<List<ConnData>> GetSortedLine()
        {
            List<List<ConnData>> allLines = new List<List<ConnData>>();

            if (line.connections.Count == 0) return allLines;

            List<MetroConnection> connections = new List<MetroConnection>(line.connections);

            while (connections.Count > 0)
            {
                MetroConnection startConn = connections[0];
                connections.RemoveAt(0);

                Vector2 currentStart = MetroRenderer.Transform(line.stations[startConn.startStationId].position);
                Vector2 currentEnd = MetroRenderer.Transform(line.stations[startConn.endStationId].position);


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
                        Vector2 start = MetroRenderer.Transform(line.stations[connection.startStationId].position);
                        Vector2 end = MetroRenderer.Transform(line.stations[connection.endStationId].position);

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
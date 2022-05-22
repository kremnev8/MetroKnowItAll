using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Util;

namespace Gameplay.MetroDisplay
{
    /// <summary>
    /// Controls how <see cref="Gameplay.MetroDisplay.Model.MetroCrossing"/> are displayed
    /// </summary>
    public class CrossingDisplay : MonoBehaviour
    {
        public MetroCrossing crossing;
        private Model.Metro metro;

        [Header("Underground")] 
        public GameObject underRoot;
        public new SpriteRenderer renderer;

        public Sprite JunctionFor2;
        public Sprite JunctionFor3;
        public Sprite JunctionFor3Triangle;
        public Sprite JunctionFor4;

        [Header("Above ground")]
        public GameObject aboveRoot;
        public Transform dotsRoot;
        public SpriteRenderer dotPrefab;
        public SpriteShapeRenderer shapeRenderer;
        public SpriteShapeController shape;

        private List<SpriteRenderer> groundDots = new List<SpriteRenderer>();
        
        private Delaunator delaunator;
        private static readonly Vector2 Up3 = Vector2.up.Rotate(-33 * Mathf.Deg2Rad);
        private static readonly int isFocused = Shader.PropertyToID("_IsFocused");
        private static MaterialPropertyBlock block;
        
        public void SetFocused(bool value)
        {
            renderer.GetPropertyBlock(block);
            block.SetFloat(isFocused, value ? 1 : 0);
            renderer.SetPropertyBlock(block);
        }
        
        public void SetCrossing(Model.Metro metro, MetroCrossing crossing)
        {
            this.metro = metro;
            this.crossing = crossing;
            
            Refresh();
        }

        public void Refresh()
        {
            bool isOpen = crossing.IsOpen(MetroRenderer.currentYear);
            gameObject.SetActive(isOpen);
            
            block ??= new MaterialPropertyBlock();
            if (isValid())
            {
                if (crossing.isAbove)
                {
                    RefreshAbove();
                }
                else
                {
                    RefreshUnder();
                }
            }
        }

        private void RefreshAbove()
        {
            List<IPoint> points = new List<IPoint>();
            transform.localPosition = Vector3.zero;
            
            foreach (SpriteRenderer groundDot in groundDots)
            {
                if (Application.isPlaying)
                {
                    Destroy(groundDot.gameObject, 0.1f);
                }
                else
                {
                    DestroyImmediate(groundDot.gameObject);
                }
            }
            groundDots.Clear();
            
            foreach (GlobalId id in crossing.stationsGlobalIds)
            {
                MetroStation station = metro.GetStation(id);
                points.Add(station.position.ToPoint());
                SpriteRenderer spriteRenderer = Instantiate(dotPrefab, dotsRoot);
                spriteRenderer.transform.position = station.position;
                groundDots.Add(spriteRenderer);
            }

            shape.spline.Clear();
            
            if (points.Count > 2)
            {
                delaunator = new Delaunator(points.ToArray());

                int i = 0;
                delaunator.ForEachTriangleEdge(edge =>
                {
                    if (shape.spline.GetPointCount() == 0)
                    {
                        shape.spline.InsertPointAt(i, edge.P.ToVector2());
                        shape.spline.SetTangentMode(i, ShapeTangentMode.Linear);
                        i++;
                    }

                    shape.spline.InsertPointAt(i, edge.Q.ToVector2().RandomizePoint(0.05f));
                    shape.spline.SetTangentMode(i, ShapeTangentMode.Linear);
                    i++;
                });
            }
            else
            {
                shape.spline.InsertPointAt(0, points[0].ToVector2());
                shape.spline.SetTangentMode(0, ShapeTangentMode.Linear);
                shape.spline.InsertPointAt(1, points[1].ToVector2());
                shape.spline.SetTangentMode(1, ShapeTangentMode.Linear);
            }

            underRoot.SetActive(false);
            aboveRoot.SetActive(true);
        }
        

        private void RefreshUnder()
        {
            bool isStraight = IsStraight();
            int count = crossing.stationsGlobalIds.Count;

            if (isStraight)
            {
                Vector2 s1 = GetPosition(crossing.stationsGlobalIds[0]);
                Vector2 s2 = GetPosition(crossing.stationsGlobalIds[1]);

                Vector2 dir = s1 - s2;

                float angle = Vector2.SignedAngle(Vector2.up, dir);
                transform.eulerAngles = new Vector3(0, 0, angle);

                if (count == 2)
                {
                    renderer.sprite = JunctionFor2;
                    transform.localScale = new Vector3(1, Mathf.Max(1, dir.magnitude / 2.5f), 1);
                }
                else if (count == 3)
                {
                    renderer.sprite = JunctionFor3;
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                if (count == 3)
                {
                    Vector2 s1 = GetPosition(crossing.stationsGlobalIds[0]);
                    Vector2 s2 = GetPosition(crossing.stationsGlobalIds[1]);

                    Vector2 dir = s1 - s2;

                    float angle = Vector2.SignedAngle(Up3, dir) - 180;
                    transform.eulerAngles = new Vector3(0, 0, angle);
                    renderer.sprite = JunctionFor3Triangle;
                }
                else if (count == 4)
                {
                    renderer.sprite = JunctionFor4;
                }
            }

            Vector2 center = GetCenter();
            transform.localPosition = center;

            underRoot.SetActive(true);
            aboveRoot.SetActive(false);
        }

        private Vector2 GetCenter()
        {
            Vector2 sum = crossing.stationsGlobalIds.Select(GetPosition).Aggregate(Vector2.zero, (current, pos) => current + pos);
            sum /= crossing.stationsGlobalIds.Count;
            return sum;
        }

        private bool IsStraight()
        {
            bool isStraight = true;

            Vector2 s1 = GetPosition(crossing.stationsGlobalIds[0]);
            Vector2 s2 = GetPosition(crossing.stationsGlobalIds[1]);

            Vector2 dir = s1 - s2;
            Vector2 normal = dir.GetNormal();

            for (int i = 2; i < crossing.stationsGlobalIds.Count; i++)
            {
                Vector2 pos = GetPosition(crossing.stationsGlobalIds[i]);
                float distance = normal.x * pos.x + normal.y * pos.y + (-normal.x * s1.x - normal.y * s1.y);
                if (Mathf.Abs(distance) > 0.2f)
                {
                    isStraight = false;
                    break;
                }
            }

            return isStraight;
        }

        private Vector2 GetPosition(GlobalId id)
        {
            return metro.lines[id.lineId].stations[id.stationId].position;
        }

        private bool isValid()
        {
            if (crossing.stationsGlobalIds.Count < 2) return false;
            
            return crossing.stationsGlobalIds.All(id =>
            {
                if (id.lineId < metro.lines.Count)
                {
                    return id.stationId < metro.lines[id.lineId].stations.Count;
                }

                return false;
            });
        }
    }
}
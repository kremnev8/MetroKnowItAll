﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Controls;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
using UnityEngine.U2D;
using Util;

namespace Gameplay.MetroDisplay
{
    /// <summary>
    /// A sub segment of <see cref="LineDisplay"/>, displaying a single continuous line segment
    /// </summary>
    public class LineSubDisplay : MonoBehaviour, ISelectable
    {
        public MetroLine line;
        public LineDisplay mainDisplay;
        private List<ConnData> points;

        [Header("Line")]
        public new SpriteShapeRenderer renderer;
        public SpriteShapeController shape;
        public Spline spline => shape.spline;

        public SpriteShape undergound;
        public SpriteShape abovetrains;

        [Header("Highlight")]
        public GameObject highlight;
        public SpriteShapeController highlightShape;
        
        private static readonly int color = Shader.PropertyToID("_Color");
        private static readonly int isFocused = Shader.PropertyToID("_IsFocused");
        private static MaterialPropertyBlock block;

        public void SetFocused(bool value)
        {
            mainDisplay.SetFocused(value);
        }
        
        
        internal void SetFocusedInternal(bool value)
        {
            renderer.GetPropertyBlock(block);
            block.SetFloat(isFocused, value ? 1 : 0);
            renderer.SetPropertyBlock(block);
        }
        
        public void Refresh()
        {
            block ??= new MaterialPropertyBlock();
            
            renderer.GetPropertyBlock(block);
            block.SetColor(color, line.lineColor);
            renderer.SetPropertyBlock(block);

            shape.spriteShape = line.style == LineStyle.UNDERGROUND ? undergound : abovetrains;

            spline.Clear();

            bool skipNext = false;
            bool isLoop = false;
            
            if (points.Count < 2) return;

            {
                Vector2 p1 = points[0].point;
                Vector2 p2 = points[points.Count - 1].point;

                if ((p1 - p2).sqrMagnitude < 0.1f)
                {
                    spline.isOpenEnded = false;
                    isLoop = true;
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1 && isLoop) break;

                bool overrideBend = points[i].connection.overrideBend;

                if (i > 0 && overrideBend)
                {
                    Vector2 p2 = points[i - 1].point;
                    Vector2 p3 = points[i].point;

                    Vector2 bendPoint = points[i].connection.bendPoint;
                    float weight = points[i].connection.weight;

                    spline.InsertPointAt(i, points[i].point);
                    spline.SetHeight(i, 1.2f);

                    spline.SetTangentMode(i - 1, ShapeTangentMode.Broken);
                    spline.SetTangentMode(i, ShapeTangentMode.Broken);

                    Vector2 d3 = bendPoint - p3;
                    float d3mag = d3.magnitude;
                    d3 = d3 / d3mag * (d3mag + weight);

                    spline.SetLeftTangent(i, d3);

                    d3 = bendPoint - p2;
                    d3mag = d3.magnitude;
                    d3 = d3 / d3mag * (d3mag + weight);

                    spline.SetRightTangent(i - 1, d3);
                    skipNext = false;
                    continue;
                }


                if (i > 2 && !line.useSmoothCurves && !skipNext)
                {
                    Vector2 p1 = points[i - 3].point;
                    Vector2 p2 = points[i - 2].point;
                    Vector2 p3 = points[i - 1].point;
                    Vector2 p4 = points[i].point;

                    overrideBend = points[i - 2].connection.overrideBend || points[i - 1].connection.overrideBend;

                    if (!overrideBend && !MathExtensions.AreParralel(p1, p2, p2, p3) &&
                        !MathExtensions.AreParralel(p2, p3, p3, p4))
                    {
                        Vector2 I = MathExtensions.IntersectLineSegments(p1, p2, p4, p3);
                        float t = MathExtensions.GetProjectionT(p2, p3, I);

                        if (t < 0)
                        {
                            if ((p2 - I).magnitude > 50)
                            {
                                I = (p2 + p3) / 2;
                            }
                            else
                            {
                                I = 2 * p2 - I;
                            }
                        }
                        else if (t > 1)
                        {
                            if ((p3 - I).magnitude > 50)
                            {
                                I = (p2 + p3) / 2;
                            }
                            else
                            {
                                I = 2 * p3 - I;
                            }
                        }

                        spline.InsertPointAt(i, points[i].point);
                        spline.SetHeight(i, 1.2f);

                        spline.SetTangentMode(i - 2, ShapeTangentMode.Broken);
                        spline.SetTangentMode(i - 1, ShapeTangentMode.Broken);
                        spline.SetTangentMode(i, ShapeTangentMode.Linear);

                        spline.SetLeftTangent(i - 1, I - p3);

                        spline.SetRightTangent(i - 2, I - p2);

                        skipNext = true;
                        continue;
                    }
                }


                spline.InsertPointAt(i, points[i].point);
                spline.SetTangentMode(i, line.useSmoothCurves ? ShapeTangentMode.Continuous : ShapeTangentMode.Linear);
                spline.SetHeight(i, 1.2f);

                skipNext = false;
            }

            if (line.useSmoothCurves)
            {
                Vector2 center = line.curveCenter;
                for (int i = 0; i < points.Count; i++)
                {
                    if (line.isLooped)
                    {
                        if (i == points.Count - 1 && isLoop) break;
                        
                        int prevIndex = (i - 1).Mod(points.Count);
                        int nextIndex = (i + 1).Mod(points.Count);
                        
                        Vector2 pos = points[i].point;
                        
                        Vector2 prevPos = points[prevIndex].point;
                        Vector2 nextPos = points[nextIndex].point;

                        float prevDist = (pos - prevPos).magnitude;
                        float nextDist = (pos - nextPos).magnitude;

                        Vector2 dir = pos - center;

                        Vector2 tan = new Vector2(dir.y, -dir.x);
                        
                        Vector2 nextTan = tan.normalized * prevDist / 4;
                        Vector2 prevTan = tan.normalized * nextDist / 4;

                        spline.SetTangentMode(i, ShapeTangentMode.Broken);
                        
                        spline.SetLeftTangent(i, nextTan);
                        spline.SetRightTangent(i, -prevTan);
                        
                    }
                    else if (i > 1)
                    {
                        int i2 = i - 2;
                        int i1 = i - 1;

                        Vector2 p2 = points[i2].point;
                        Vector2 p4 = points[i].point;

                        Vector2 dir = p2 - p4;
                        dir /= 8;

                        spline.SetLeftTangent(i1, dir);
                        spline.SetRightTangent(i1, -dir);
                    }
                }
            }

            highlightShape.spline.CopyValues(spline);
            for (int i = 0; i < highlightShape.spline.GetPointCount(); i++)
            {
                highlightShape.spline.SetCornerMode(i, Corner.Disable);
            }

            shape.BakeMeshForced();
            highlightShape.BakeMeshForced();
        }
        
        
        public void SetGroupData(List<ConnData> _points, LineDisplay display)
        { 
            points = _points;
            line = display.line;
            mainDisplay = display;

            Refresh();
#if UNITY_EDITOR
            Invoke(nameof(InternalHideHighlight), 0.2f);
#endif
        }

#if UNITY_EDITOR
        private void InternalHideHighlight()
        {
            if (!Application.isPlaying)
            {
                highlight.SetActive(false);
            }
        }
#endif

        public bool IsFocused(MetroRenderer metroRenderer)
        {
            if (metroRenderer.focusRegion.regionType == RegionType.GLOBAL_LINE)
            {
                return metroRenderer.focusRegion.lineId == line.lineId;
            }

            return line.stations.Any(station => metroRenderer.focusRegion.Contains(station));
        }

        public void SetSelected(MetroRenderer metroRenderer, bool value)
        {
            highlight.SetActive(value);
        }

        public void ShowLabel(bool visible)
        {
        }

        private void Start()
        {
            Invoke(nameof(HideOnStart), 0.3f);
        }

        private void HideOnStart()
        {
            highlight.SetActive(false);
        }
    }
}
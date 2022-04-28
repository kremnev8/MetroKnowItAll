using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gameplay;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Util
{

    public static class MathExtensions
    {
        public static Vector2 Rotate(this Vector2 v, float delta) {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

        public static Vector2 ClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float xDelta = p2.x - p1.x;
            float yDelta = p2.y - p1.y;

            if (xDelta == 0 && yDelta == 0) return p3;

            float u = ((p3.x - p1.x) * xDelta + (p3.y - p1.y) * yDelta) / (xDelta * xDelta + yDelta * yDelta);

            return new Vector2(p1.x + u * xDelta, p1.y + u * yDelta);
        }

        public static Vector3 ClampHorizontal(this Vector3 vector, float maxMagnitude)
        {
            float y = vector.y;
            vector.y = 0;
            if (vector.sqrMagnitude > maxMagnitude * maxMagnitude) vector = vector.normalized * maxMagnitude;

            vector.y = y;
            return vector;
        }

        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 CopyVector(this Vector2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }

        public static Vector2 ToVector2(this Vector3Int vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }
        
        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static bool Less(this Vector3 vec1, Vector3 vec2)
        {
            return vec1.x <= vec2.x && vec1.y <= vec2.y && vec1.z <= vec2.z;
        }

        public static bool Greater(this Vector3 vec1, Vector3 vec2)
        {
            return vec1.x >= vec2.x && vec1.y >= vec2.y && vec1.z >= vec2.z;
        }
        
        public static bool Less(this Vector2 vec1, Vector2 vec2)
        {
            return vec1.x <= vec2.x && vec1.y <= vec2.y;
        }

        public static bool Greater(this Vector2 vec1, Vector2 vec2)
        {
            return vec1.x >= vec2.x && vec1.y >= vec2.y;
        }

        public static Vector3 GetBiggestAxis(this Vector3 vector)
        {
            float x = Mathf.Abs(vector.x);
            float y = Mathf.Abs(vector.y);
            float z = Mathf.Abs(vector.z);

            if (x > y && x > z) return new Vector3(vector.x, 0, 0);

            if (y > x && y > z) return new Vector3(0, vector.y, 0);

            if (z > x && z > y) return new Vector3(0, 0, vector.z);

            return Vector3.zero;
        }

        public static Vector2 GetNormal(this Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x);
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static bool Approximately(float a, float b, float tolerance = 1e-5f)
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        public static float CrossProduct2D(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        public static bool AreParralel(Vector2 p1start, Vector2 p1end, Vector2 p2start, Vector2 p2end)
        {
            Vector2 r = p1end - p1start;
            Vector2 s = p2end - p2start;

            float cross_rs = CrossProduct2D(r.normalized, s.normalized);

            return Approximately(cross_rs, 0f, 0.05f);
        }

        public static float GetProjectionT(Vector2 a, Vector2 b, Vector2 point)
        {
            Vector2 d = b - a;
            float tu = (point.x - a.x) * (b.x - a.x) + (point.y - a.y) * (b.y - a.y);

            return tu / d.sqrMagnitude;
        }


        /// <summary>
        /// Find intersection of 2 2D lines
        /// </summary>
        /// <param name="p1start">Start point of the first line</param>
        /// <param name="p1end">End point of the first line</param>
        /// <param name="p2start">Start point of the second line</param>
        /// <param name="p2end">End point of the second line</param>
        /// <returns>Intersection point</returns>
        public static Vector2 IntersectLineSegments(Vector2 p1start, Vector2 p1end, Vector2 p2start, Vector2 p2end)
        {
            Vector2 r = p1end - p1start;
            Vector2 s = p2end - p2start;

            float cross_rs = CrossProduct2D(r, s);
            Vector2 qminusp = p2start - p1start;

            float t = CrossProduct2D(qminusp, s) / cross_rs;
            return p1start + t * r;
        }

        public static Vector2 Apply(this Vector2 vec, Func<float, float> func)
        {
            return new Vector2(func(vec.x), func(vec.y));
        }
        
        public static Vector2Int Apply(this Vector2 vec, Func<float, int> func)
        {
            return new Vector2Int(func(vec.x), func(vec.y));
        }

        public static string GetDirection(this Vector2 dir)
        {
            Vector2Int intDir = dir.normalized.Apply(Mathf.RoundToInt);

            return intDir.x switch
            {
                1 => intDir.y switch
                {
                    1 => "северо-восточнее",
                    -1 => "юго-восточнее",
                    _ => "восточнее"
                },
                -1 => intDir.y switch
                {
                    1 => "северо-западнее",
                    -1 => "юго-западнее",
                    _ => "западнее"
                },
                _ => intDir.y switch
                {
                    1 => "севернее",
                    -1 => "южнее",
                    _ => "возле"
                }
            };
        }

        public static Vector2 GetCenter(this IList<Vector2> points)
        {
            return points.Aggregate((a, b) => a + b) / points.Count;
        }
        
        public static Vector2 GetCenter(this IEnumerable<Vector2> points, int count)
        {
            return points.Aggregate((a, b) => a + b) / count;
        }

        public static bool IsInfinity(this Vector2 vector)
        {
            return float.IsInfinity(vector.x) || float.IsInfinity(vector.y);
        }
        
        public static int Mod(this int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
        
        public static void Shuffle<T>(this IList<T> list)  
        {
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = Random.Range(0, n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
        
        
        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInPolygon4(this List<Vector2> polygon, Vector2 testPoint)
        {
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].y < testPoint.y && polygon[j].y >= testPoint.y || polygon[j].y < testPoint.y && polygon[i].y >= testPoint.y)
                {
                    if (polygon[i].x + (testPoint.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < testPoint.x)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
        
        public static bool IsClockwise(this List<Vector2> vertices)
        {
            double sum = 0.0;
            Vector2 v1 = vertices[vertices.Count - 1];
            
            foreach (Vector2 v2 in vertices)
            {
                sum += (v2.x - v1.x) * (v2.y + v1.y);
                v1 = v2;
            }
            return sum > 0.0;
        }
        
        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side and then restricts the given vector to the resulting rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The vector, clamped to the Rect.</returns>
        public static Vector2 Clamp2(this Rect rect, Vector2 position, float extendDistance = 0f)
        {
            return new Vector2(Mathf.Clamp(position.x, rect.xMin - extendDistance, rect.xMax + extendDistance),
                Mathf.Clamp(position.y, rect.yMin - extendDistance, rect.yMax + extendDistance));
        }
        
        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side and then restricts the given vector to the resulting rect.
        /// The z component is kept.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The vector, clamped to the Rect.</returns>
        public static Vector3 Clamp3(this Rect rect, Vector3 position, float extendDistance = 0f)
        {
            return new Vector3(Mathf.Clamp(position.x, rect.xMin - extendDistance, rect.xMax + extendDistance),
                Mathf.Clamp(position.y, rect.yMin - extendDistance, rect.yMax + extendDistance),
                position.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  static float Average(this float current, float newValue, int total)
        {
            return current * (total - 1) / total + newValue / total;
        }
    }
}
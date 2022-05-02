using System.Linq;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
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

        public new SpriteRenderer renderer;

        public Sprite JunctionFor2;
        public Sprite JunctionFor3;
        public Sprite JunctionFor3Triangle;
        public Sprite JunctionFor4;

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
            block ??= new MaterialPropertyBlock();
            if (isValid())
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
                    }else if (count == 3)
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
                    }else if (count == 4)
                    {
                        renderer.sprite = JunctionFor4;
                    }
                }

                Vector2 center = GetCenter();
                transform.localPosition = center;
            }
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
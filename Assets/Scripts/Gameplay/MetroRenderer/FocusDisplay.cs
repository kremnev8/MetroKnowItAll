using System.Collections.Generic;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
using UnityEngine.U2D;
using Util;

namespace Gameplay.MetroDisplay
{
    public class FocusDisplay : MonoBehaviour
    {
        public Region region;
        private MetroRenderer metroRenderer;
        
        [Header("Shape")]
        [SerializeField] private new SpriteShapeRenderer renderer;
        [SerializeField] private SpriteShapeController shape;
        
        private Spline spline => shape.spline;

        [SerializeField] private SpriteShape fill;
        [SerializeField] private SpriteShape outline;

        public void SetRegion(MetroRenderer metroRenderer, Region region)
        {
            this.metroRenderer = metroRenderer;
            this.region = region;
            
            Refresh(true);
        }
        
        public void Refresh(bool refreshShape)
        {
            if (refreshShape)
            {
                spline.Clear();
                spline.isOpenEnded = false;
                List<Vector2> points = region.area.points;

                if (region.area.points.IsClockwise())
                {
                    points.Reverse();
                }

                for (int i = 0; i < points.Count; i++)
                {
                    Vector2 point = points[i];
                    spline.InsertPointAt(i, point);
                    spline.SetTangentMode(i, ShapeTangentMode.Broken);
                    spline.SetCornerMode(i, Corner.Stretched);
                }
            }
            
            if (metroRenderer.focusRegion.regionType != RegionType.GLOBAL)
            {
                bool isFocused = region.regionType == metroRenderer.focusRegion.regionType;
                shape.spriteShape = isFocused ? outline : fill;
                renderer.enabled = true;
            }
            else
            {
                renderer.enabled = false;
            }
        }
    }
}
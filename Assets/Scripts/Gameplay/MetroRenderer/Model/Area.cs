using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public struct Area
    {
        public static readonly Area Everywhere = new Area(200, 200, -200, -200);
        
        public Vector2 startPos;
        public Vector2 endPos;

        public Area(float startx, float starty, float endx, float endy)
        {
            startPos = new Vector2(startx, starty);
            endPos = new Vector2(endx, endy);
        }
        
        public Area(Vector2 start, Vector2 end)
        {
            startPos = start;
            endPos = end;
        }

        [Pure]
        public Vector4 GetVector()
        {
            return new Vector4(startPos.x, startPos.y, endPos.x,endPos.y);
        }

        public bool IsInside(Vector2 pos)
        {
            return pos.x < startPos.x && endPos.x < pos.x && pos.y < startPos.y && endPos.y < pos.y;
        }
        
    }
}
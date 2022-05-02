using UnityEngine;

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Text alignment options
    /// </summary>
    public enum NamePosition
    {
        DEFAULT,
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT,
        CENTER_LEFT,
        CENTER_RIGHT
    }
    

    public static class EnumHelper
    {
        public static Vector2 GetDirection(this NamePosition position)
        {
            return position switch
            {
                NamePosition.DEFAULT => new Vector2(2, 0),
                NamePosition.TOP_LEFT => new Vector2(-1, 1),
                NamePosition.TOP_CENTER => new Vector2(0, 1),
                NamePosition.TOP_RIGHT => new Vector2(1, 1),
                NamePosition.BOTTOM_LEFT => new Vector2(-1, -1),
                NamePosition.BOTTOM_CENTER => new Vector2(0, -1),
                NamePosition.BOTTOM_RIGHT => new Vector2(1, -1),
                NamePosition.CENTER_LEFT => new Vector2(-2, 0),
                NamePosition.CENTER_RIGHT => new Vector2(2, 0),
                _ => new Vector2(0, 0)
            };
        }
        
        public static Vector2 GetOffset(this NamePosition position)
        {
            return position switch
            {
                NamePosition.DEFAULT => new Vector2(0, -0.5f),
                NamePosition.TOP_LEFT => new Vector2(-1, 0),
                NamePosition.TOP_CENTER => new Vector2(-0.5f, 0),
                NamePosition.TOP_RIGHT => new Vector2(0, 0),
                NamePosition.BOTTOM_LEFT => new Vector2(-1, -1),
                NamePosition.BOTTOM_CENTER => new Vector2(-0.5f, -1),
                NamePosition.BOTTOM_RIGHT => new Vector2(0, -1),
                NamePosition.CENTER_LEFT => new Vector2(-1, -0.5f),
                NamePosition.CENTER_RIGHT => new Vector2(0, -0.5f),
                _ => new Vector2(0, 0)
            };
        }
    }
}
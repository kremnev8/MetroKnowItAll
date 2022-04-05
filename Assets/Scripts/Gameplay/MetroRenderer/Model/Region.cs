using System;

namespace Gameplay
{
    [Serializable]
    public struct Region
    {
        public RegionType regionType;
        public int lineId;

    }

    public enum RegionType
    {
        GLOBAL = 0,
        CENTER,
        NORTH,
        NORTH_EAST,
        EAST,
        SOUTH_EAST,
        SOUTH,
        SOUTH_WEST,
        WEST,
        NORTH_WEST
    }
}
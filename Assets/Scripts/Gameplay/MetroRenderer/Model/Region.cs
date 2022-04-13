using System;

namespace Gameplay
{
    [Serializable]
    public struct Region
    {
        public RegionType regionType;
        public int lineId;
        public Area area;

        public Region(RegionType type, Area area, int lineId)
        {
            regionType = type;
            this.area = area;
            this.lineId = lineId;
        }

        public string GetName(Metro metro)
        {
            return regionType switch
            {
                RegionType.GLOBAL => metro.lines[lineId].name.Replace("линия", ""),
                RegionType.CENTER => "Центр",
                RegionType.NORTH => "Сервер",
                RegionType.NORTH_EAST => "Северо-восток",
                RegionType.EAST => "Восток",
                RegionType.SOUTH_EAST => "Юго-восток",
                RegionType.SOUTH => "Юг",
                RegionType.SOUTH_WEST => "Юго-запад",
                RegionType.WEST => "Запад",
                RegionType.NORTH_WEST => "Северо-запад",
                _ => ""
            };
        }
    }

    public enum RegionType
    {
        GLOBAL = 0,
        CENTER = 1,
        NORTH = 2,
        NORTH_EAST = 3,
        EAST = 4,
        SOUTH_EAST = 5,
        SOUTH = 6,
        SOUTH_WEST = 7,
        WEST = 8,
        NORTH_WEST = 9,
        MAX_VALUE = 10
    }
}
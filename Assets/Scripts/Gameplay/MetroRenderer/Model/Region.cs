using System;
using System.Linq;
using UnityEngine;
using Util;

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Defines a region of a metro, which can be a line, an area or the whole metro
    /// </summary>
    [Serializable]
    public class Region
    {
        public static Region everywhere => new Region(RegionType.GLOBAL, Area.Everywhere, -1);
        
        public RegionType regionType;
        public int lineId;
        public Area area;

        public Region(RegionType type, Area area, int lineId)
        {
            regionType = type;
            this.area = area;
            this.lineId = lineId;
        }

        public bool Contains(MetroStation station)
        {
            if (regionType == RegionType.GLOBAL)
            {
                return lineId == station.lineId;
            }

            if (lineId != -1)
            {
                return lineId == station.lineId && regionType == station.regionType;
            }
            return regionType == station.regionType;
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
        
        public static string GetFileName(RegionType regionType)
        {
            return regionType switch
            {
                RegionType.GLOBAL => "",
                RegionType.CENTER => "ЦЕНТРАЛЬНЫЙ ОКРУГ",
                RegionType.NORTH => "СЕВЕРНЫЙ ОКРУГ",
                RegionType.NORTH_EAST => "СЕВЕРО-ВОСТОЧНЫЙ ОКРУГ",
                RegionType.EAST => "ВОСТОЧНЫЙ ОКРУГ",
                RegionType.SOUTH_EAST => "ЮГО-ВОСТОЧНЫЙ ОКРУГ",
                RegionType.SOUTH => "ЮЖНЫЙ ОКРУГ",
                RegionType.SOUTH_WEST => "ЮГО-ЗАПАДНЫЙ ОКРУГ",
                RegionType.WEST => "ЗАПАДНЫЙ ОКРУГ",
                RegionType.NORTH_WEST => "СЕВЕРО-ЗАПАДНЫЙ ОКРУГ",
                _ => ""
            };
        }

        public Vector2 GetRegionCenter(Metro metro)
        {
            if (regionType == RegionType.GLOBAL)
            {
                MetroLine line = metro.lines[lineId];
                return line.stations.Select(station => station.position).GetCenter(line.stations.Count);
            }

            return area.points.GetCenter();
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
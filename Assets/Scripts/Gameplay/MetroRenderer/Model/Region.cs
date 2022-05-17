using System;
using System.Collections.Generic;
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
        public static Region everywhere => new Region(RegionType.GLOBAL_LINE, -1);
        
        public RegionType regionType;
        public int lineId;
        public HashSet<int> stations;
        
        public Region(RegionType type, HashSet<int> stations, int lineId)
        {
            regionType = type;
            this.stations = stations;
            this.lineId = lineId;
            if (this.stations == null)
            {
                this.stations = new HashSet<int>();
            }
            
        }
        
        public Region(RegionType type, int lineId)
        {
            regionType = type;
            stations = new HashSet<int>();
            this.lineId = lineId;
        }

        public bool Contains(MetroStation station)
        {
            if (lineId == -1 && regionType != RegionType.GLOBAL_LINE && regionType != RegionType.GLOBAL_STATIONS)
            {
                return StationsContain(station.globalId) && regionType == station.regionType;
            }
            
            if (lineId == -1 && regionType == RegionType.GLOBAL_STATIONS)
            {
                return StationsContain(station.globalId);
            }

            if (regionType != RegionType.GLOBAL_LINE && regionType != RegionType.GLOBAL_STATIONS)
            {
                return regionType == station.regionType && lineId == station.lineId && StationsContain(station.globalId);
            }
            
            return lineId == station.lineId && StationsContain(station.globalId);
        }

        private bool StationsContain(GlobalId globalId)
        {
            if (stations == null || stations.Count == 0)
            {
                return true;
            }
            else
            {
                return stations.Contains(globalId);
            }
        }
        
        public string GetName(Metro metro)
        {
            return regionType switch
            {
                RegionType.GLOBAL_LINE => metro.lines[lineId].name.Replace("линия", ""),
                RegionType.GLOBAL_STATIONS => "Изученные станций",
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
                RegionType.GLOBAL_LINE => "",
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
            if (regionType == RegionType.GLOBAL_LINE && lineId != -1)
            {
                MetroLine line = metro.lines[lineId];
                return line.stations.Select(station => station.position).GetCenter(line.stations.Count);
            }

            if (stations != null && stations.Count > 0)
            {
                return stations.Select(id => metro.GetStation(id).position).GetCenter(stations.Count);
            }

            Vector2 center = Vector2.zero;
            int count = 0;
            foreach (MetroLine line in metro.lines)
            {
                foreach (MetroStation station in line.stations) 
                {
                    if (station.regionType == regionType)
                    {
                        center += station.position;
                        count++;
                    }
                }
            }

            return center / count;
        }

        protected bool Equals(Region other)
        {
            if (stations != null && other.stations != null)
            {
                return regionType == other.regionType && lineId == other.lineId && stations.SetEquals(other.stations);
            }
            
            if (stations == null && other.stations == null)
            {
                return regionType == other.regionType && lineId == other.lineId;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Region)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)regionType;
                hashCode = (hashCode * 397) ^ lineId;
                hashCode = (hashCode * 397) ^ (stations != null ? stations.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public enum RegionType
    {
        GLOBAL_LINE = 0,
        GLOBAL_STATIONS = 11,
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
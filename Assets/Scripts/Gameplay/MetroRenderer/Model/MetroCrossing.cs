using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Class that combines line and station id's
    /// </summary>
    [Serializable]
    public class GlobalId : IEquatable<GlobalId>, IComparable<GlobalId>
    {
        public byte lineId;
        public byte stationId;
        public GlobalId(int lineId, int stationId)
        {
            this.lineId = (byte)lineId;
            this.stationId = (byte)stationId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(GlobalId id) => id.lineId * 100 + id.stationId;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GlobalId(int number) => new GlobalId(number / 100, number % 100);

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(GlobalId lhs, GlobalId rhs)
        {
            if (lhs is null || rhs is null) return false;
            return lhs.Equals(rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(GlobalId lhs, GlobalId rhs) => !(lhs == rhs);

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            
            if (obj is int num)
            {
                return num == lineId * 100 + stationId;
            }
            
            if (obj is GlobalId id)
            {
                return lineId == id.lineId && stationId == id.stationId;
            }

            return false;
        }

        public bool Equals(GlobalId other)
        {
            return lineId == other.lineId && stationId == other.stationId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (lineId.GetHashCode() * 397) ^ stationId.GetHashCode();
            }
        }

        public int CompareTo(GlobalId other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            int lineIdComparison = lineId.CompareTo(other.lineId);
            if (lineIdComparison != 0) return lineIdComparison;
            return stationId.CompareTo(other.stationId);
        }
    }
    
    /// <summary>
    /// Defines a crossing between lines
    /// </summary>
    [Serializable]
    public class MetroCrossing
    {
        public List<GlobalId> stationsGlobalIds;
        
        public bool isAbove;

        public int openIn;
        public int closedIn;
    }
}
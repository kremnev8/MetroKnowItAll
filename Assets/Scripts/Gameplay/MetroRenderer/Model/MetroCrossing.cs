using System;
using System.Collections.Generic;

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Class that combines line and station id's
    /// </summary>
    [Serializable]
    public class GlobalId
    {
        public byte lineId;
        public byte stationId;
    }
    
    /// <summary>
    /// Defines a crossing between lines
    /// </summary>
    [Serializable]
    public class MetroCrossing
    {
        public List<GlobalId> stationsGlobalIds;

        public bool isOpen;
    }
}
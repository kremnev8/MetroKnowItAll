using System;
using System.Collections.Generic;

namespace Gameplay.MetroDisplay.Model
{
    [Serializable]
    public class GlobalId
    {
        public byte lineId;
        public byte stationId;
    }
    
    [Serializable]
    public class MetroCrossing
    {
        public List<GlobalId> stationsGlobalIds;

        public bool isOpen;
    }
}
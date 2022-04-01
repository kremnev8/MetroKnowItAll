using System;
using System.Collections.Generic;

namespace Gameplay
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
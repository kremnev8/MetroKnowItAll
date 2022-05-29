using System;

namespace Gameplay.MetroDisplay.Model
{
    
    /// <summary>
    /// Defines <see cref="MetroStation"/> which are the same, but have different <see cref="GlobalId"/>
    /// </summary>
    [Serializable]
    public class MetroSiblings
    {
        public GlobalId[] stations;
    }
}
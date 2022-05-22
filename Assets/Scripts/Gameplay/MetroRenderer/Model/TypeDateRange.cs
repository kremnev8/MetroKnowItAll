using System;
using UnityEngine.Serialization;

namespace Gameplay.MetroDisplay.Model
{
    [Serializable]
    public struct TypeDateRange<T>
    {
        [FormerlySerializedAs("name")]
        public T value;

        public int openIn;
        public int closedIn;

        public TypeDateRange(T value, int openIn, int closedIn)
        { 
            this.value = value;
            this.openIn = openIn;
            this.closedIn = closedIn;
        }
        
        public bool InScope(int year)
        {
            return year >= openIn && year < closedIn;
        }
    }
}
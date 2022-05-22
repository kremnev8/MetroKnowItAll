using System;

namespace Gameplay.MetroDisplay.Model
{
    [Serializable]
    public struct NameDateRange
    {
        public string name;

        public int openIn;
        public int closedIn;

        public NameDateRange(string name, int openIn, int closedIn)
        { 
            this.name = name;
            this.openIn = openIn;
            this.closedIn = closedIn;
        }
    }
}
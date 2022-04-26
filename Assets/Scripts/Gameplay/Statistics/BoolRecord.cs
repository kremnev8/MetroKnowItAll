using System.Collections.Generic;

namespace Gameplay.Statistics
{
    public interface IIndexable<out T>
    {
        T index { get; }
    }
    
    public class BoolRecord<T, TIndex>
    where T : IIndexable<TIndex>
    {
        public Dictionary<TIndex, bool> data = new Dictionary<TIndex, bool>();

        public void Prepare(IEnumerable<T> allEntries)
        {
            foreach (T entry in allEntries)
            {
                data[entry.index] = false;
            }
        }
        
        public void Unlock(T station)
        {
            data[station.index] = true;
        }
        
        public bool IsUnlocked(T station)
        {
            if (data.ContainsKey(station.index))
            {
                return data[station.index];
            }

            return false;
        }
    }
}
using System.Collections.Generic;

namespace Gameplay.Statistics
{
    /// <summary>
    /// Generic interface for index-able types
    /// </summary>
    /// <typeparam name="T">Index type</typeparam>
    public interface IIndexable<out T>
    {
        T index { get; }
    }
    
    /// <summary>
    /// Bool record for any index-able type
    /// </summary>
    /// <typeparam name="T">Source type</typeparam>
    /// <typeparam name="TIndex">Index type</typeparam>
    public class BoolRecord<T, TIndex>
    where T : IIndexable<TIndex>
    {
        public Dictionary<TIndex, bool> data = new Dictionary<TIndex, bool>();

        /// <summary>
        /// Initialize the record with intial data
        /// </summary>
        /// <param name="allEntries"></param>
        public void Prepare(IEnumerable<T> allEntries)
        {
            foreach (T entry in allEntries)
            {
                data[entry.index] = false;
            }
        }
        
        /// <summary>
        /// Unlock a record
        /// </summary>
        /// <param name="station"></param>
        public void Unlock(T station)
        {
            data[station.index] = true;
        }
        
        /// <summary>
        /// Is record unlocked?
        /// </summary>
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
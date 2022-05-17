using System.Collections.Generic;
using Gameplay.MetroDisplay.Model;

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
        public HashSet<TIndex> data = new HashSet<TIndex>();
        
        
        /// <summary>
        /// Unlock a record
        /// </summary>
        /// <param name="station"></param>
        public void Unlock(T station)
        {
            data.Add(station.index);
        }
        
        /// <summary>
        /// Is record unlocked?
        /// </summary>
        public bool IsUnlocked(T station)
        {
            return data.Contains(station.index);
        }
    }
}
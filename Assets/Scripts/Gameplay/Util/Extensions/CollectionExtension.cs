using System.Collections.Generic;
using Gameplay.MetroDisplay.Model;

namespace Util
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Utility method to try get a value from dictionary.
        /// If dictionary has no such key, a default value is returned
        /// </summary>
        public static TV TryGet<TK, TV>(this Dictionary<TK, TV> dictionary, TK key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return default;
        }
        
        /// <summary>
        /// Utility method to try set a value in dictionary.
        /// If dictionary has no such key, it is added
        /// </summary>
        public static void TrySet<TK, TV>(this Dictionary<TK, TV> dictionary, TK key, TV value)
        { 
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        
        public static bool IsOpen(this DateRange[] range, int year)
        {
            foreach (DateRange entry in range)
            {
                if (year >= entry.openIn && year <= entry.closedIn)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetCurrentName(this NameDateRange[] range, int year)
        {
            foreach (NameDateRange entry in range)
            {
                if (year >= entry.openIn && year <= entry.closedIn)
                {
                    return entry.name;
                }
            }

            return "Нет Имени!";
        }
    }
}
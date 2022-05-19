using System.Collections.Generic;

namespace Util
{
    public static class DictionaryExtension
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
    }
}
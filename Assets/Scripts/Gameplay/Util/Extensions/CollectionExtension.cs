﻿using System.Collections.Generic;
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

        public static T GetCurrent<T>(this List<TypeDateRange<T>> range, int year)
        {
            foreach (TypeDateRange<T> entry in range)
            {
                if (year >= entry.openIn && year < entry.closedIn)
                {
                    return entry.value;
                }
            }

            return default;
        }

        public static void Insert<T>(this List<TypeDateRange<T>> list, int year, T value)
        {
            int index = list.FindIndex(range => range.InScope(year));
            TypeDateRange<T> original = list[index];
            TypeDateRange<T> first = new TypeDateRange<T>(original.value, original.openIn, year);
            TypeDateRange<T> second = new TypeDateRange<T>(value, year, original.closedIn);
            list[index] = first;
            list.Insert(index + 1, second);
        }

        public static void Remove<T>(this List<TypeDateRange<T>> list, int year)
        {
            int startIndex = list.FindIndex(range => range.closedIn == year);
            int endIndex = list.FindIndex(range => range.openIn == year);

            TypeDateRange<T> first = list[startIndex];
            first.closedIn = list[endIndex].closedIn;
            list[startIndex] = first;
            list.RemoveAt(endIndex);
        }
    }
}
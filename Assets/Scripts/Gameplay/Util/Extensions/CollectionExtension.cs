using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gameplay.MetroDisplay.Model;
using UnityEngine;

namespace Util
{
    /// <summary>
    /// Utility methods for collections
    /// </summary>
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

        /// <summary>
        /// Evaluate history range and find current value
        /// </summary>
        /// <param name="range">history range</param>
        /// <param name="year">current year</param>
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

        /// <summary>
        /// Modify history range to insert a new change
        /// </summary>
        /// <param name="list">history range</param>
        /// <param name="year">insert year</param>
        /// <param name="value">new value</param>
        public static void Insert<T>(this List<TypeDateRange<T>> list, int year, T value)
        {
            int index = list.FindIndex(range => range.InScope(year));
            TypeDateRange<T> original = list[index];
            TypeDateRange<T> first = new TypeDateRange<T>(original.value, original.openIn, year);
            TypeDateRange<T> second = new TypeDateRange<T>(value, year, original.closedIn);
            list[index] = first;
            list.Insert(index + 1, second);
        }

        /// <summary>
        /// Modify history range to remove a change
        /// </summary>
        /// <param name="list">history range</param>
        /// <param name="year">target year</param>
        public static void Remove<T>(this List<TypeDateRange<T>> list, int year)
        {
            int startIndex = list.FindIndex(range => range.closedIn == year);
            int endIndex = list.FindIndex(range => range.openIn == year);

            TypeDateRange<T> first = list[startIndex];
            first.closedIn = list[endIndex].closedIn;
            list[startIndex] = first;
            list.RemoveAt(endIndex);
        }

        public static void AddAll<T>(this HashSet<int> set, List<TypeDateRange<T>> list)
        {
            foreach (TypeDateRange<T> entry in list)
            {
                set.Add(entry.openIn);
                set.Add(entry.closedIn);
            }
        }

        /// <summary>
        /// Get a fixed amount of stations, from start. Skips any closed stations.
        /// </summary>
        /// <param name="stations">list of all stations</param>
        /// <param name="startIndex">start index</param>
        /// <param name="count">station count</param>
        /// <exception cref="ArgumentException">List does not contains count stations which are open</exception>
        public static List<MetroStation> GetStationRange(this List<MetroStation> stations, int startIndex, int count)
        {
            List<MetroStation> result = new List<MetroStation>(count);
            for (int i = startIndex; i < startIndex + count; i++)
            {
                MetroStation station = stations[i];
                if (station.isOpen)
                {
                    result.Add(station);
                    continue;
                }

                if (startIndex + count < stations.Count)
                {
                    count++;
                }
                else
                {
                    throw new ArgumentException($"Can't get range of stations from {startIndex}, to {startIndex + count} because the list is too small!");
                }
            }

            return result;
        }
        
        /// <summary>
        /// Get an attribute of type T from a MemberInfo
        /// </summary>
        /// <param name="memberInfo">target object info</param>
        /// <param name="customAttribute">result attribute</param>
        /// <typeparam name="T">attribute type</typeparam>
        /// <returns>was an attribute found?</returns>
        public static bool TryGetAttribute<T>(this MemberInfo memberInfo, out T customAttribute) where T: Attribute {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            if (attributes == null) {
                customAttribute = null;
                return false;
            }
            customAttribute = (T)attributes;
            return true;
        }
    }
}
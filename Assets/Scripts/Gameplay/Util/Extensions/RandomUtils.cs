using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Util
{
    /// <summary>
    /// Helper methods to generate random numbers
    /// </summary>
    public static class RandomUtils
    {
        /// <summary>
        /// Generate a number between min and max with a filter
        /// </summary>
        /// <param name="filter">Filter function, if function returns false for a number, it will not be included into random set</param>
        /// <exception cref="System.ArgumentException"> thrown if all numbers were excluded</exception>
        public static int ConstrainedRandom(Func<int, bool> filter, int min, int max)
        {
            int[] items = Enumerable.Range(min, max - min).Where(filter).ToArray();

            if (items.Length == 0) throw new ArgumentException("Can't pick random item, nothing left!");
            return items[Random.Range(0, items.Length)];
        }

        /// <summary>
        /// Generate a number between min and max with a filter
        /// </summary>
        /// <param name="blacklist">Any numbers in this list will no be added to random set</param>
        /// <exception cref="System.ArgumentException"> thrown if all numbers were excluded</exception>
        public static int ConstrainedRandom(this List<int> blacklist, int min, int max)
        {
            int[] items = Enumerable.Range(min, max - min).Where(i => !blacklist.Contains(i)).ToArray();

            if (items.Length == 0) throw new ArgumentException("Can't pick random item, nothing left!");
            return items[Random.Range(0, items.Length)];
        }

        /// <summary>
        /// Generate a number between min and max excluding a single number
        /// </summary>
        /// <exception cref="System.ArgumentException"> thrown if all numbers were excluded</exception>
        public static int ConstrainedRandom(this byte exclude, int min, int max)
        {
            return ConstrainedRandom(num => num != exclude, min, max);
        }

        /// <summary>
        /// Generate a number between min and max excluding a single number
        /// </summary>
        /// <exception cref="System.ArgumentException"> thrown if all numbers were excluded</exception>
        public static int ConstrainedRandom(this int exclude, int min, int max)
        {
            return ConstrainedRandom(num => num != exclude, min, max);
        }

        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = Random.value * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;
            }

            return default(T);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Util
{
    public static class RandomUtils
    {
        public static int ConstrainedRandom(Func<int, bool> filter, int min, int max)
        {
            int[] items = Enumerable.Range(min, max - min).Where(filter).ToArray();

            if (items.Length == 0) throw new ArgumentException("Can't pick random item, nothing left!");
            return items[Random.Range(0, items.Length)];
        }

        public static int ConstrainedRandom(this List<int> blacklist, int min, int max)
        {
            int[] items = Enumerable.Range(min, max - min).Where(i => !blacklist.Contains(i)).ToArray();
            
            if (items.Length == 0) throw new ArgumentException("Can't pick random item, nothing left!");
            return items[Random.Range(0, items.Length)];
        }

        public static int ConstrainedRandom(this byte exclude, int min, int max)
        {
            return ConstrainedRandom((int)exclude, min, max);
        }

        public static int ConstrainedRandom(this int exclude, int min, int max)
        {
            int[] items = Enumerable.Range(min, max - min).Where(i => i != exclude).ToArray();
            
            if (items.Length == 0) throw new ArgumentException("Can't pick random item, nothing left!");
            return items[Random.Range(0, items.Length)];
        }
    }
}
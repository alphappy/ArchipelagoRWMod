using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace alphappy.Archipelago
{
    internal static class Extensions
    {
        /// <summary>
        /// If this Queue is not empty, dequeue and supply that; otherwise, supply default and return false.
        /// </summary>
        internal static bool TryPop<T>(this Queue<T> self, out T item)
        {
            if (self.Count == 0)
            {
                item = default; 
                return false;
            }
            item = self.Dequeue();
            return true;
        }

        public static T Pick<T>(this IEnumerable<T> list)
        {
            if (list.Count() == 0)
                return default;

            return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
        }

        public static T2 GetOrDefault<T1, T2>(this Dictionary<T1, T2> self, T1 key, T2 defaultValue = default)
        {
            if (self.TryGetValue(key, out T2 value)) return value;
            return defaultValue;
        }
    }
}

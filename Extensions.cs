using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace alphappy.Archipelago
{
    /// <summary>
    /// Generic extensions for convenience.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Try to dequeue from this <see cref="Queue{T}"/>.
        /// </summary>
        /// <typeparam name="T">The contained type of the <see cref="Queue{T}"/>.</typeparam>
        /// <param name="self">The <see cref="Queue{T}"/>.</param>
        /// <param name="item">The result of <see cref="Queue{T}.Dequeue"/>, or <see langword="default"/> if it is empty.</param>
        /// <returns><see langword="true"/> if <paramref name="self"/> had an item to dequeue, and <see langword="false"/> otherwise.</returns>
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
        /// <summary>
        /// Pick a random item from this <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The contained type of the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>A random element from this <see cref="IEnumerable{T}"/>, or <see langword="default"/> if it is empty.</returns>
        public static T Pick<T>(this IEnumerable<T> list)
        {
            if (list.Count() == 0)
                return default;

            return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
        }

        /// <summary>
        /// Tries to get the value associated with a particular key and defaults if the key isn't in the dictionary.
        /// </summary>
        /// <typeparam name="T1">The key type of the <see cref="Dictionary{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="T2">The value type of the <see cref="Dictionary{TKey, TValue}"/>.</typeparam>
        /// <param name="self">The <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="key">The key to look for.</param>
        /// <param name="defaultValue">The value to return if the key isn't in the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <returns>The value associated with <paramref name="key"/> if it is in the dictionary; otherwise, <paramref name="defaultValue"/>.</returns>
        public static T2 GetOrDefault<T1, T2>(this Dictionary<T1, T2> self, T1 key, T2 defaultValue = default)
        {
            if (self.TryGetValue(key, out T2 value)) return value;
            return defaultValue;
        }
    }
}

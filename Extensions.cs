using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alphappy.Archipelago
{
    internal static class Extensions
    {
        internal static bool Pop<T>(this Queue<T> self, out T item)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Util
{
    public static class ListUtil
    {
        // Can be made more efficient by only iterating to the random chosen element
        public static T Random<T>(this IEnumerable<T> list)
        {
            var array = list as T[] ?? list.ToArray();
            var size = array.Length;

            return array.ElementAt(UnityEngine.Random.Range(0, size));
        }

        public static T Random<T>(this IEnumerable<T> list, Func<T, bool> predicate) {
            var array = list as T[] ?? list.Where(predicate).ToArray();
            var size = array.Length;

            return array.ElementAt(UnityEngine.Random.Range(0, size));
        }

    }
}

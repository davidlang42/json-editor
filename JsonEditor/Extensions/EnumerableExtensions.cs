﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor.Extensions
{
    internal static class EnumerableExtensions
    {
        /// <summary>Like SingleOrDefault() except returns null if the sequence contains more than 1 element.</summary>
        public static T? SingleOrDefaultSafe<T>(this IEnumerable<T> sequence) where T : class
        {
            var e = sequence.GetEnumerator();
            if (!e.MoveNext())
                return null;
            var first = e.Current;
            if (e.MoveNext())
                return null;
            return first;
        }

        /// <summary>Like SingleOrDefault() except returns null if the sequence contains more than 1 element.</summary>
        public static (K, V)? SingleOrDefaultSafe<K, V>(this IEnumerable<KeyValuePair<K, V>> sequence)
        {
            var e = sequence.GetEnumerator();
            if (!e.MoveNext())
                return null;
            var first = e.Current;
            if (e.MoveNext())
                return null;
            return (first.Key, first.Value);
        }

        public static IEnumerable<T> Yield<T>(this T obj)
        {
            yield return obj;
        }
    }
}

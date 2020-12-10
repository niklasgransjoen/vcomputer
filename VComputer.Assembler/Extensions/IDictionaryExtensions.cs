using System;
using System.Collections.Generic;
using System.Text;

namespace VComputer.Assembler.Extensions
{
    internal static class IDictionaryExtensions
    {
        public static void AddRange<TKey1, TValue1, TKey2, TValue2>(
            this IDictionary<TKey1, TValue1> d1,
            IEnumerable<KeyValuePair<TKey2, TValue2>> d2)
            where TKey1 : notnull
            where TKey2 : TKey1
            where TValue2 : TValue1
        {
            foreach (var pair in d2)
            {
                d1.Add(pair.Key, pair.Value);
            }
        }
    }
}

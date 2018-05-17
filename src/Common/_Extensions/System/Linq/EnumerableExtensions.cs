// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using KodeAid;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
        {
            ArgCheck.NotNull(nameof(source), source);
            var partition = new List<T>(size);
            foreach (var item in source)
            {
                partition.Add(item);
                if (partition.Count == size)
                {
                    yield return partition;
                    partition = new List<T>(size);
                }
            }
            if (partition.Count > 0)
            {
                yield return partition;
            }
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source, int size)
        {
            ArgCheck.NotNull(nameof(source), source);

            // https://math.stackexchange.com/a/130862
            //Remark: For any fixed k, the number of k-element subsets of a set of n items is n!/k!(n−k)!
            // https://stackoverflow.com/questions/127704/algorithm-to-return-all-combinations-of-k-elements-from-n
            return size == 0 ? new[] { new T[0] } :
              source.SelectMany((e, i) =>
                source.Skip(i + 1).Combinations(size - 1).Select(c => (new[] { e }).Concat(c)));
        }

        public static ISet<T> ToSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
    }
}

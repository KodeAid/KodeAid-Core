// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Linq;
using KodeAid;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            ArgCheck.NotNull(nameof(collection), collection);

            if (items != null)
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            ArgCheck.NotNull(nameof(collection), collection);

            if (items != null)
            {
                foreach (var item in items)
                {
                    collection.Remove(item);
                }
            }
        }

        public static void ForEach<T>(this ICollection<T> collection, Action<T> action)
        {
            ArgCheck.NotNull(nameof(collection), collection);
            ArgCheck.NotNull(nameof(action), action);

            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static void RemoveAll<T>(this ICollection<T> collection, Predicate<T> match)
        {
            ArgCheck.NotNull(nameof(collection), collection);
            ArgCheck.NotNull(nameof(match), match);

            collection.RemoveRange(collection.Where(item => match(item)).ToList());
        }
    }
}

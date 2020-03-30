// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            ArgCheck.NotNull(nameof(collection), collection);
            if (items != null)
                foreach (var item in items)
                    collection.Add(item);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            ArgCheck.NotNull(nameof(collection), collection);
            if (items != null)
                foreach (var item in items)
                    collection.Remove(item);
        }
    }
}

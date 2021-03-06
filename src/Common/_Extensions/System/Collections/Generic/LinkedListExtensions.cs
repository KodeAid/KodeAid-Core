// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System.Collections.Generic
{
    public static class LinkedListExtensions
    {
        public static void AddRange<T>(this LinkedList<T> linkedList, IEnumerable<T> items)
        {
            ArgCheck.NotNull(nameof(linkedList), linkedList);
            if (items != null)
                foreach (var item in items)
                    linkedList.AddLast(item);
        }
    }
}

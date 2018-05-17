// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System.Collections.Generic
{
    public static class QueueExtensions
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            ArgCheck.NotNull(nameof(queue), queue);
            if (queue != null && items != null)
                foreach (var item in items)
                    queue.Enqueue(item);
        }
    }
}

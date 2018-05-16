// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace KodeAid.Collections.ObjectModel
{
    public class Grouping<TKey, T> : BulkObservableCollection<T>
    {
        public Grouping(TKey key, IEnumerable<T> items)
        {
            ArgCheck.NotNull(nameof(items), items);
            Key = key;
            foreach (var item in items)
                Items.Add(item);
        }

        public TKey Key { get; }
    }
}

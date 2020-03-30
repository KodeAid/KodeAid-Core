// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Caching
{
    public class CacheResult<T> : ICacheResult<T>
    {
        public CacheResult()
        {
        }

        public CacheResult(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
        public bool IsHit { get; set; }
        public T Value { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }

        public override string ToString()
        {
            return $"{Key} = {(IsHit ? "HIT" : "MISS")}";
        }
    }
}

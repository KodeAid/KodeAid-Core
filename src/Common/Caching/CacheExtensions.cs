// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KodeAid.Caching
{
    public static class CacheExtensions
    {
        public static Task<ICacheResult<T>> GetAsync<T>(this ICache cache, object key, string partition = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(cache), cache);
            return cache.GetAsync<T>(key?.ToString(), partition);
        }

        public static Task<IEnumerable<ICacheResult<T>>> GetRangeAsync<T>(this ICache cache, IEnumerable keys, string partition = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(cache), cache);
            ArgCheck.NotNull(nameof(keys), keys);
            return cache.GetRangeAsync<T>(keys.OfType<object>().Select(key => key?.ToString()), partition);
        }

        public static Task SetAsync<T>(this ICache cache, object key, T value, DateTimeOffset? absoluteExpiration = null, string partition = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(cache), cache);
            return cache.SetAsync(key?.ToString(), value, absoluteExpiration, partition);
        }

        public static Task SetAsync<T>(this ICache cache, Func<T, object> getKey, T value, DateTimeOffset? absoluteExpiration = null, string partition = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(cache), cache);
            ArgCheck.NotNull(nameof(getKey), getKey);
            return SetAsync(cache, getKey(value), value, absoluteExpiration, partition);
        }

        public static Task SetRangeAsync<T>(this ICache cache, Func<T, object> getKey, IEnumerable<T> values, DateTimeOffset? absoluteExpiration = null, string partition = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(cache), cache);
            ArgCheck.NotNull(nameof(getKey), getKey);
            ArgCheck.NotNull(nameof(values), values);
            var pairs = values.Select(value => new KeyValuePair<string, T>(getKey(value).ToString(), value)).ToList();
            return cache.SetRangeAsync(pairs, absoluteExpiration, partition);
        }
    }
}

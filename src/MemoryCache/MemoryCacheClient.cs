// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MSMemoryCache = System.Runtime.Caching.MemoryCache;

namespace KodeAid.Caching.MemoryCache
{
    public class MemoryCacheClient : CacheClientBase
    {
        private static MSMemoryCache _client;

        public MemoryCacheClient(string name, ILogger<MemoryCacheClient> logger, bool throwOnError = false)
          : base(throwOnError, logger)
        {
            ArgCheck.NotNull(nameof(name), name);
            _client = new MSMemoryCache(name);
        }

        protected override Task<IEnumerable<CacheItem<T>>> GetItemsAsync<T>(IEnumerable<string> keys, string partition)
        {
            if (!string.IsNullOrEmpty(partition))
                keys = keys.Select(key => $"${partition}$|{key}").ToList();
            return Task.FromResult(_client.GetValues(keys, null).Select(pair => (CacheItem<T>)pair.Value).ToList().AsEnumerable());
        }

        protected override Task SetItemsAsync<T>(IEnumerable<CacheItem<T>> items, string partition)
        {
            var utcNow = DateTimeOffset.UtcNow;
            foreach (var item in items)
            {
                var key = item.Key;
                if (!string.IsNullOrEmpty(partition))
                {
                    key = $"${partition}$|{key}";
                }
                if (item.Expiration.HasValue)
                {
                    _client.Set(key, item, item.Expiration.Value, null);
                }
                else
                {
                    _client.Set(key, item, null, null);
                }
            }
            return Task.CompletedTask;
        }

        protected override Task RemoveKeysAsync(IEnumerable<string> keys, string partition = null)
        {
            if (!string.IsNullOrEmpty(partition))
                keys = keys.Select(key => $"${partition}$|{key}").ToList();
            foreach (var key in keys)
            {
                _client.Remove(key, CacheEntryRemovedReason.Removed, null);
            }
            return Task.CompletedTask;
        }
    }
}

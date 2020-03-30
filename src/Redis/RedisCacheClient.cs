// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KodeAid.Serialization;
using KodeAid.Serialization.Binary;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace KodeAid.Caching.Redis
{
    public class RedisCacheClient : CacheClientBase
    {
        private static ConcurrentDictionary<string, ConnectionMultiplexer> _clientLookup = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        private ConnectionMultiplexer _client;
        private readonly ISerializer _serializer;

        public RedisCacheClient(string connectionString, ILogger<RedisCacheClient> logger, bool throwOnError = false)
            : this(connectionString, new DotNetBinarySerializer(), logger, throwOnError)
        {
        }

        public RedisCacheClient(string connectionString, ISerializer serializer, ILogger<RedisCacheClient> logger, bool throwOnError = false)
            : base(throwOnError, logger)
        {
            ArgCheck.NotNullOrEmpty(nameof(connectionString), connectionString);
            ArgCheck.NotNull(nameof(serializer), serializer);
            _client = _clientLookup.GetOrAdd(connectionString, key => ConnectionMultiplexer.Connect(connectionString));
            _serializer = serializer;
        }

        protected override async Task<IEnumerable<CacheItem<T>>> GetItemsAsync<T>(IEnumerable<string> keys, string partition)
        {
            if (!string.IsNullOrEmpty(partition))
            {
                keys = keys.Select(key => $"${partition}$|{key}").ToList();
            }
            var hits = await _client.GetDatabase().StringGetAsync(keys.Cast<RedisKey>().ToArray()).ConfigureAwait(false);
            if (hits == null)
            {
                return null;
            }
            return hits.Where(hit => !hit.IsNull && hit.HasValue).Select(value => DeserializeItem<T>(value)).ToList();
        }

        protected override async Task SetItemsAsync<T>(IEnumerable<CacheItem<T>> items, string partition)
        {
            var utcNow = DateTime.UtcNow;
            foreach (var groupOfExpiries in items.GroupBy(item => item.Expiration))
            {
                var database = _client.GetDatabase();
                if (!groupOfExpiries.Key.HasValue)
                {
                    await database.StringSetAsync(groupOfExpiries.Select(item => new KeyValuePair<RedisKey, RedisValue>(
                      !string.IsNullOrEmpty(partition) ? $"${partition}$|{item.Key}" : item.Key,
                      SerializeItem(item))).ToArray()).ConfigureAwait(false);
                }
                else
                {
                    var ttl = groupOfExpiries.Key.Value - utcNow;
                    if (ttl > TimeSpan.Zero)
                    {
                        foreach (var item in groupOfExpiries)
                        {
                            var key = item.Key;
                            if (!string.IsNullOrEmpty(partition))
                            {
                                key = $"${partition}$|{key}";
                            }
                            await database.StringSetAsync(key, SerializeItem(item), ttl).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        protected override async Task RemoveKeysAsync(IEnumerable<string> keys, string partition = null)
        {
            if (!string.IsNullOrEmpty(partition))
                keys = keys.Select(key => $"${partition}$|{key}");
            await _client.GetDatabase().KeyDeleteAsync(keys.Select(key => (RedisKey)key).ToArray()).ConfigureAwait(false);
        }

        private RedisValue SerializeItem<T>(CacheItem<T> item)
        {
            return (RedisValue)_serializer.Serialize(item);
        }

        private CacheItem<T> DeserializeItem<T>(RedisValue value)
        {
            return _serializer.Deserialize<CacheItem<T>>(value);
        }
    }
}

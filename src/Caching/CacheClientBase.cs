﻿// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using KodeAid.Linq;
using KodeAid.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProtoBuf;

namespace KodeAid.Caching
{
    public abstract class CacheClientBase : ICache
    {
        public CacheClientBase(bool throwOnError, ILogger logger)
        {
            ThrowOnError = throwOnError;
            Logger = logger ?? NopLogger.Instance;
        }

        protected bool ThrowOnError { get; }
        protected ILogger Logger { get; }

        public virtual async Task<ICacheResult<T>> GetAsync<T>(string key, string regionName = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(key), key);
            var results = await GetAsyncHelper<T>(new[] { key }, regionName).ConfigureAwait(false);
            return results.Single();
        }

        public virtual Task<IEnumerable<ICacheResult<T>>> GetRangeAsync<T>(IEnumerable<string> keys, string regionName = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(keys), keys);
            if (keys.Any(key => key == null))
            {
                throw new ArgumentException("Parameter keys must not contain null items.", "keys");
            }
            return GetAsyncHelper<T>(keys, regionName);
        }

        public virtual Task SetAsync<T>(string key, T value, string regionName = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(key), key);
            return SetAsyncHelper(new[] { new KeyValuePair<string, T>(key, value) }, null, regionName);
        }

        public virtual Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration, string regionName = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(key), key);
            return SetAsyncHelper(new[] { new KeyValuePair<string, T>(key, value) }, absoluteExpiration, regionName);
        }

        public virtual Task SetRangeAsync<T>(IEnumerable<KeyValuePair<string, T>> keyValues, string regionName = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(keyValues), keyValues);
            if (keyValues.Any(keyValue => keyValue.Key == null))
            {
                throw new ArgumentException("Parameter keyValues must contain non-null keys.", "keyValues");
            }
            if (keyValues.GroupBy(keyValue => keyValue.Key).Any(g => g.Count() > 1))
            {
                throw new ArgumentException("Parameter keyValues must contain only distinct keys.", "keyValues");
            }
            return SetAsyncHelper(keyValues, null, regionName);
        }

        public virtual Task SetRangeAsync<T>(IEnumerable<KeyValuePair<string, T>> keyValues, DateTimeOffset absoluteExpiration, string regionName = null)
            where T : new()
        {
            ArgCheck.NotNull(nameof(keyValues), keyValues);
            if (keyValues.Any(keyValue => keyValue.Key == null))
            {
                throw new ArgumentException("Parameter keyValues must contain non-null keys.", "keyValues");
            }
            if (keyValues.GroupBy(keyValue => keyValue.Key).Any(g => g.Count() > 1))
            {
                throw new ArgumentException("Parameter keyValues must contain only distinct keys.", "keyValues");
            }
            return SetAsyncHelper(keyValues, absoluteExpiration, regionName);
        }

        public virtual Task RemoveAsync(string key, string regionName = null)
        {
            ArgCheck.NotNull(nameof(key), key);
            return RemoveRangeAsync(EnumerableHelper.From(key), regionName);
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<string> keys, string regionName = null)
        {
            ArgCheck.NotNull(nameof(keys), keys);
            keys = keys.Where(key => key != null).ToList();
            if (!keys.Any())
                return;
            var sw = new Stopwatch();
            sw.Start();
            await RemoveKeysAsync(keys, regionName);
            sw.Stop();
            if (keys.Count() == 1)
            {
                Logger.LogInformation("Cache removed key '{0}' in {1}ms.", keys.FirstOrDefault(), sw.ElapsedMilliseconds);
            }
            else
            {
                Logger.LogInformation("Cache removed {0} keys in {1}ms.", keys.Count(), sw.ElapsedMilliseconds);
            }
        }

        protected abstract Task<IEnumerable<CacheItem<T>>> GetItemsAsync<T>(IEnumerable<string> keys, string regionName) where T : new();

        protected abstract Task SetItemsAsync<T>(IEnumerable<CacheItem<T>> items, string regionName) where T : new();

        protected virtual Task RemoveKeysAsync(IEnumerable<string> keys, string regionName = null)
        {
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<ICacheResult<T>>> GetAsyncHelper<T>(IEnumerable<string> keys, string regionName)
            where T : new()
        {
            try
            {
                if (keys.Any())
                {
                    var distinctKeys = keys.Distinct().ToList();
                    var sw = new Stopwatch();
                    sw.Start();
                    var items = await GetItemsAsync<T>(distinctKeys, regionName).ConfigureAwait(false);
                    sw.Stop();
                    if (items != null)
                    {
                        var results = items
                            .Where(item => item != null && (!item.Expiration.HasValue || item.Expiration.Value > DateTimeOffset.UtcNow))
                            .Select(item => new CacheResult<T>(item.Key) { IsHit = true, Value = item.Value, LastUpdated = item.LastUpdated });
                        if (distinctKeys.Count == 1)
                        {
                            if (results.Count() == 1)
                            {
                                Logger.LogInformation("Cache hit on key '{0}' in {1}ms.", distinctKeys[0], sw.ElapsedMilliseconds);
                                return keys.Select(key => results.Single()).ToList();
                            }
                            else
                            {
                                Logger.LogInformation("Cache miss on key '{0}' in {1}ms.", distinctKeys[0], sw.ElapsedMilliseconds);
                                return keys.Select(key => new CacheResult<T>(key)).ToList();
                            }
                        }
                        else
                        {
                            var resultLookup = results.ToDictionary(result => result.Key);

                            // logging
                            if (resultLookup.Count == 0)
                            {
                                Logger.LogInformation("Cache miss on all {0} keys in {1}ms.", distinctKeys.Count, sw.ElapsedMilliseconds);
                            }
                            else if (resultLookup.Count == distinctKeys.Count)
                            {
                                Logger.LogInformation("Cache hit on all {0} keys in {1}ms.", distinctKeys.Count, sw.ElapsedMilliseconds);
                            }
                            else
                            {
                                Logger.LogInformation("Cache hit on {0} keys and miss on {1} keys of total {2} keys in {3}ms.",
                                  resultLookup.Count, distinctKeys.Count - resultLookup.Count, distinctKeys.Count, sw.ElapsedMilliseconds);
                            }

                            return keys.Select(key =>
                            {
                                if (!resultLookup.TryGetValue(key, out CacheResult<T> result) || result == null)
                                {
                                    result = new CacheResult<T>(key);
                                    resultLookup[key] = result;
                                }
                                return result;
                            }).ToList();
                        }
                    }
                    else
                    {
                        Logger.LogInformation("Cache miss on all {0} keys in {1}ms.", distinctKeys.Count, sw.ElapsedMilliseconds);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get keys from cache: {0}.", string.Join(", ", keys));
                if (ThrowOnError)
                {
                    throw;
                }
            }
            return keys.Select(key => new CacheResult<T>(key)).ToList();
        }

        private async Task SetAsyncHelper<T>(IEnumerable<KeyValuePair<string, T>> keyValues, DateTimeOffset? absoluteExpiration, string regionName)
            where T : new()
        {
            var utcNow = DateTimeOffset.UtcNow;
            if (absoluteExpiration.HasValue)
            {
                absoluteExpiration = absoluteExpiration.Value.ToUniversalTime();
                if (absoluteExpiration.Value <= utcNow)
                {
                    return;
                }
            }

            var items = keyValues.Select(keyValue => 
                new CacheItem<T>()
                {
                    Key = keyValue.Key,
                    Value = keyValue.Value,
                    Expiration = absoluteExpiration,
                    LastUpdated = utcNow
                }).ToList();

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                await SetItemsAsync(items, regionName).ConfigureAwait(false);
                sw.Stop();

                // logging
                if (items.Count == 1)
                {
                    if (absoluteExpiration.HasValue)
                    {
                        Logger.LogInformation("Cache set on key '{0}' in {1}ms expirying at {2}.",
                            items[0].Key, sw.ElapsedMilliseconds, absoluteExpiration.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        Logger.LogInformation("Cache set on key '{0}' in {1}ms.", items[0].Key, sw.ElapsedMilliseconds);
                    }
                }
                else
                {
                    if (absoluteExpiration.HasValue)
                    {
                        Logger.LogInformation("Cache set on '{0}' keys in {1}ms expirying at {2}.",
                            items.Count, sw.ElapsedMilliseconds, absoluteExpiration.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        Logger.LogInformation("Cache set on '{0}' keys in {1}ms.", items.Count, sw.ElapsedMilliseconds);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to set cache for keys: {0}.", string.Join(", ", keyValues.Select(keyValue => keyValue.Key)));
                if (ThrowOnError)
                {
                    throw;
                }
            }
        }

        [Serializable]
        [DataContract]
        [JsonObject]
        [ProtoContract]
        public class CacheItem<T>
        {
            [DataMember(IsRequired = true)]
            [JsonProperty("key", Required = Required.Always)]
            [ProtoMember(1, IsRequired = true)]
            public string Key { get; set; }

            [DataMember(IsRequired = true)]
            [JsonProperty("value", Required = Required.Always)]
            [ProtoMember(2, IsRequired = true)]
            public T Value { get; set; }

            [DataMember]
            [JsonProperty("expiry", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            [ProtoMember(3)]
            public DateTimeOffset? Expiration { get; set; }

            [DataMember]
            [JsonProperty("updated", Required = Required.Always)]
            [ProtoMember(4)]
            public DateTimeOffset LastUpdated { get; set; }
        }
    }
}
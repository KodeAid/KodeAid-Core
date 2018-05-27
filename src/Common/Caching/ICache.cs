// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KodeAid.Caching
{
    public interface ICache
    {
        Task<ICacheResult<T>> GetAsync<T>(string key, string regionName = null) where T : new();
        Task<IEnumerable<ICacheResult<T>>> GetRangeAsync<T>(IEnumerable<string> keys, string regionName = null) where T : new();
        Task SetAsync<T>(string key, T value, string regionName = null) where T : new();
        Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration, string regionName = null) where T : new();
        Task SetRangeAsync<T>(IEnumerable<KeyValuePair<string, T>> keyValues, string regionName = null) where T : new();
        Task SetRangeAsync<T>(IEnumerable<KeyValuePair<string, T>> keyValues, DateTimeOffset absoluteExpiration, string regionName = null) where T : new();
        Task RemoveAsync(string key, string regionName = null);
        Task RemoveRangeAsync(IEnumerable<string> keys, string regionName = null);
    }
}

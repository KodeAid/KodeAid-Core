// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KodeAid.Caching
{
    public interface ICache
    {
        Task<ICacheResult<T>> GetAsync<T>(string key, string partition = null) where T : new();
        Task<IEnumerable<ICacheResult<T>>> GetRangeAsync<T>(IEnumerable<string> keys, string partition = null) where T : new();
        Task SetAsync<T>(string key, T value, DateTimeOffset? absoluteExpiration = null, string partition = null) where T : new();
        Task SetRangeAsync<T>(IEnumerable<KeyValuePair<string, T>> keyValues, DateTimeOffset? absoluteExpiration = null, string partition = null) where T : new();
        Task RemoveAsync(string key, string partition = null);
        Task RemoveRangeAsync(IEnumerable<string> keys, string partition = null);
    }
}

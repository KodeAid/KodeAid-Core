// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Storage
{
    public interface IKeyValueStore : IKeyValueReadOnlyStore
    {
        Task<object> AddOrReplaceAsync(string key, string value, string partition = null, object concurrencyToken = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default);
        Task<object> AddOrReplaceAsync(string key, byte[] bytes, string partition = null, object concurrencyToken = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default);
        Task<object> AddOrReplaceAsync(string key, Stream stream, string partition = null, object concurrencyToken = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, string partition = null, CancellationToken cancellationToken = default);
    }
}

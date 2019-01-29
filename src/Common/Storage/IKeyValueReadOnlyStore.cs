// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Storage
{
    public interface IKeyValueReadOnlyStore
    {
        Task<IStringResult> GetStringAsync(string key, string partition = null, object concurrencyStamp = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default);
        Task<IBytesResult> GetBytesAsync(string key, string partition = null, object concurrencyStamp = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default);
        Task<IStreamResult> GetStreamAsync(string key, string partition = null, object concurrencyStamp = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default);
    }
}

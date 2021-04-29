// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Storage
{
    public interface IDataStore
    {
        Task<bool> ExistsAsync(string name, string partition = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<IBlobMeta>> ListAsync(string directoryRelativeAddress = null, CancellationToken cancellationToken = default);
        Task<IBlobResult> GetAsync(string name, string partition = null, object concurrencyToken = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default);
        Task<object> AddOrReplaceAsync(BlobData blob, CancellationToken cancellationToken = default);
        Task RemoveAsync(string name, string partition = null, CancellationToken cancellationToken = default);
    }
}

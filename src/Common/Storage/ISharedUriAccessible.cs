// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Storage
{
    public interface ISharedUriAccessible
    {
        Task<Uri> GetSharedUriAsync(string name, string partition = null, AccessPermissions permissions = AccessPermissions.Read, DateTimeOffset? startTime = null, DateTimeOffset? expiryTime = null, CancellationToken cancellationToken = default);
    }
}

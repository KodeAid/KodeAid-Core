// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Security.Secrets
{
    public interface ISecretReadOnlyStore
    {
        Task<SecureString> GetSecretAsync(string name, CancellationToken cancellationToken = default);
    }
}

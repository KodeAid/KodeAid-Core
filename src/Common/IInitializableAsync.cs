// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;

namespace KodeAid
{
    public interface IInitializableAsync
    {
        bool IsInitialized { get; }
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}

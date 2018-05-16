// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;

namespace KodeAid
{
    public interface IAsyncInitializable
    {
        bool IsInitialized { get; }
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}

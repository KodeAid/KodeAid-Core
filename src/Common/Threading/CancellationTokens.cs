// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading;

namespace KodeAid.Threading
{
    public static class CancellationTokens
    {
        public static readonly CancellationToken CancelledToken = new CancellationToken(true);
        public static readonly CancellationToken InfiniteToken = new CancellationToken(false);
    }
}

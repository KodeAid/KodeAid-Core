// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading;

namespace KodeAid.Threading
{
    public static class CancellationTokens
    {
        public static readonly CancellationToken CancelledToken = new CancellationToken(true);
        public static readonly CancellationToken InfiniteToken = new CancellationToken(false);
    }
}

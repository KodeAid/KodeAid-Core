// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace System.Threading
{
    public static class CancellationTokenExtensions
    {
        public static bool WaitOnCancellationRequested(this CancellationToken cancellationToken, TimeSpan timeout)
        {
            return cancellationToken.WaitHandle.WaitOne(timeout);
        }
    }
}

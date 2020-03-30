// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


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

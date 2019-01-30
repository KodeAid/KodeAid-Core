// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.FaultTolerance
{
    public class RetryContext
    {
        protected internal RetryContext()
        {
        }

        private volatile bool _canRetry;
        private volatile int _retryCount;

        public bool CanRetry { get => _canRetry; protected internal set => _canRetry = value; }
        public int RetryCount { get => _retryCount; protected internal set => _retryCount = value; }
    }
}

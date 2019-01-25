// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.FaultTolerance
{
    public class RetryManager<T> : RetryManagerBase<T>
    {
        private readonly Func<RetryContext, T, Exception, bool> _canRetry;

        public RetryManager(RetryPolicy policy, Func<RetryContext, T, Exception, bool> canRetry)
            : base(policy)
        {
            ArgCheck.NotNull(nameof(canRetry), canRetry);

            _canRetry = canRetry;
        }

        protected override bool CanRetry(RetryContext context, T state, Exception exception)
        {
            return _canRetry.Invoke(context, state, exception);
        }
    }
}

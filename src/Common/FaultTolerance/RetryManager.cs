// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.FaultTolerance
{
    public class RetryManager<TState> : RetryManagerBase<TState>
    {
        private readonly Func<RetryContext, TState, Exception, bool> _canRetry;

        public RetryManager(IRetryPolicy policy, Func<RetryContext, TState, Exception, bool> canRetry)
            : base(policy)
        {
            ArgCheck.NotNull(nameof(canRetry), canRetry);

            _canRetry = canRetry;
        }

        protected override bool CanRetry(RetryContext context, TState state, Exception exception)
        {
            return _canRetry.Invoke(context, state, exception);
        }
    }
}

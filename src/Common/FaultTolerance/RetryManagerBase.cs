// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public abstract class RetryManagerBase<TState>
    {
        public RetryManagerBase(IRetryPolicy policy)
        {
            ArgCheck.NotNull(nameof(policy), policy);

            Policy = policy;
        }

        public IRetryPolicy Policy { get; }

        public async Task<(bool Retry, RetryContext Context)> CheckRetryAsync(RetryContext context, TState state = default, Exception exception = null)
        {
            if (!CanRetry(context, state, exception))
            {
                return (false, context);
            }

            return await Policy.RetryDelayAsync(context).ConfigureAwait(false);
        }

        protected abstract bool CanRetry(RetryContext context, TState state, Exception exception);
    }
}

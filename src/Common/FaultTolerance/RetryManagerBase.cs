// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public abstract class RetryManagerBase<T>
    {
        public RetryManagerBase(RetryPolicy policy)
        {
            ArgCheck.NotNull(nameof(policy), policy);

            Policy = policy;
        }

        public RetryPolicy Policy { get; }

        public async Task<(bool Retry, RetryContext Context)> CheckRetryAsync(RetryContext context, T state = default, Exception exception = null)
        {
            if (!CanRetry(context, state, exception))
            {
                return (false, context);
            }

            return await Policy.RetryDelayAsync(context).ConfigureAwait(false);
        }

        protected abstract bool CanRetry(RetryContext context, T state, Exception exception);
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public abstract class RetryPolicy : IRetryPolicy
    {
        public int MaxRetryCount { get; set; } = 3;
        public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

        public virtual RetryContext CreateRetryContext()
        {
            return new RetryContext();
        }

        public virtual async Task<bool> CheckRetryAndDelayAsync(RetryContext context, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(context), context);

            if (context.RetryCount >= MaxRetryCount)
            {
                return false;
            }

            await DelayAsync(context, cancellationToken).ConfigureAwait(false);

            context.RetryCount++;
            return true;
        }

        protected virtual Task DelayAsync(RetryContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}

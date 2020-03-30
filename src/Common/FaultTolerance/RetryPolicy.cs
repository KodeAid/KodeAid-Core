// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public abstract class RetryPolicy : IRetryPolicy
    {
        protected virtual int? GetMaxRetryCount() => null;

        public virtual RetryContext CreateRetryContext() => new RetryContext();

        public virtual async Task<bool> CheckRetryAndDelayAsync(RetryContext context, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(context), context);

            var maxRetryCount = GetMaxRetryCount();

            if (maxRetryCount.HasValue && context.RetryCount >= maxRetryCount.Value)
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

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public abstract class OperationManager
    {
        public OperationManager(IRetryPolicy retryPolicy = null)
        {
            RetryPolicy = retryPolicy;
        }

        public IRetryPolicy RetryPolicy { get; }

        public virtual OperationContext CreateOperationContext(object state = null)
        {
            return new OperationContext(state, RetryPolicy?.CreateRetryContext());
        }

        public async virtual Task ExecuteOperationAsync(Func<Task> operation, object state = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(operation), operation);

            var context = CreateOperationContext(state);

            while (true)
            {
                try
                {
                    await operation().ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    var status = await ProcessOperationAsync(context, null, ex, cancellationToken).ConfigureAwait(false);
                    if (status != OperationStatus.Retry)
                    {
                        throw;
                    }
                }
            }
        }

        public async virtual Task<TResult> ExecuteOperationAsync<TResult>(Func<Task<TResult>> operation, object state = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(operation), operation);

            var context = CreateOperationContext(state);

            while (true)
            {
                try
                {
                    var result = await operation().ConfigureAwait(false);
                    var status = await ProcessOperationAsync(context, result, null, cancellationToken).ConfigureAwait(false);
                    if (status != OperationStatus.Retry)
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    var status = await ProcessOperationAsync(context, null, ex, cancellationToken).ConfigureAwait(false);
                    if (status != OperationStatus.Retry)
                    {
                        throw;
                    }
                }
            }
        }

        public async virtual Task<OperationStatus> ProcessOperationAsync(OperationContext context, object result, Exception exception, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(context), context);

            if (CheckIsSuccess(context.State, result, exception))
            {
                return OperationStatus.Success;
            }

            if (RetryPolicy == null)
            {
                return OperationStatus.RetryDisabled;
            }

            if (!CheckIsRetryable(context.State, result, exception))
            {
                return OperationStatus.NonRetryable;
            }

            await RetryPolicy.CheckRetryAndDelayAsync(context.RetryContext, cancellationToken).ConfigureAwait(false);
            if (context.RetryContext.CanRetry)
            {
                return OperationStatus.Retry;
            }

            return OperationStatus.RetryExhausted;
        }

        protected virtual bool CheckIsSuccess(object state, object result, Exception exception)
        {
            return false;
        }

        protected virtual bool CheckIsRetryable(object state, object result, Exception exception)
        {
            return false;
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public abstract class OperationManager : OperationManager<object>
    {
        public OperationManager(IRetryPolicy retryPolicy = null)
            : base(retryPolicy)
        {
        }
    }

    public abstract class OperationManager<TStatusCode>
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

        public Task ExecuteOperationAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            return ExecuteOperationAsync(operation, null, cancellationToken);
        }

        public async virtual Task ExecuteOperationAsync(Func<Task> operation, object state, CancellationToken cancellationToken = default)
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
                    var operationStatus = await ProcessOperationAsync(context, default, ex, cancellationToken).ConfigureAwait(false);
                    if (operationStatus != OperationStatus.Retry)
                    {
                        throw;
                    }
                }
            }
        }

        public Task<TResult> ExecuteOperationAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            return ExecuteOperationAsync(operation, null, cancellationToken);
        }

        public async virtual Task<TResult> ExecuteOperationAsync<TResult>(Func<Task<TResult>> operation, object state, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(operation), operation);

            var context = CreateOperationContext(state);

            while (true)
            {
                try
                {
                    var result = await operation().ConfigureAwait(false);
                    var statusCode = GetStatusCodeFromResult(result);
                    var operationStatus = await ProcessOperationAsync(context, statusCode, null, cancellationToken).ConfigureAwait(false);
                    if (operationStatus != OperationStatus.Retry)
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    var status = await ProcessOperationAsync(context, default, ex, cancellationToken).ConfigureAwait(false);
                    if (status != OperationStatus.Retry)
                    {
                        throw;
                    }
                }
            }
        }

        public async virtual Task<OperationStatus> ProcessOperationAsync(OperationContext context, TStatusCode statusCode, Exception exception, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(context), context);

            if (CheckIsSuccess(context.State, statusCode, exception))
            {
                return OperationStatus.Success;
            }

            if (RetryPolicy == null)
            {
                return OperationStatus.RetryDisabled;
            }

            if (!CheckIsRetryable(context.State, statusCode, exception))
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

        protected virtual bool CheckIsSuccess(object state, TStatusCode statusCode, Exception exception)
        {
            return exception == null;
        }

        protected virtual bool CheckIsRetryable(object state, TStatusCode statusCode, Exception exception)
        {
            return false;
        }

        protected virtual TStatusCode GetStatusCodeFromResult(object result)
        {
            return result is TStatusCode ? (TStatusCode)result : default;
        }
    }
}

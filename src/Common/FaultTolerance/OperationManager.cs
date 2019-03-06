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
                    var operationStatus = await ProcessOperationAsync(context, GetDefaultStatusCode(), null, cancellationToken).ConfigureAwait(false);
                    if (operationStatus != RetryStatus.Retry)
                    {
                        context.SetCompleted(true);
                        ExecuteOperationCompleted(context);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    var operationStatus = await ProcessOperationAsync(context, GetDefaultStatusCode(), ex, cancellationToken).ConfigureAwait(false);
                    if (operationStatus != RetryStatus.Retry)
                    {
                        context.SetCompleted(false);
                        ExecuteOperationCompleted(context);
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
                    if (operationStatus != RetryStatus.Retry)
                    {
                        context.SetCompleted(true);
                        ExecuteOperationCompleted(context);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    var status = await ProcessOperationAsync(context, default, ex, cancellationToken).ConfigureAwait(false);
                    if (status != RetryStatus.Retry)
                    {
                        context.SetCompleted(false);
                        ExecuteOperationCompleted(context);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Called after an operation has completed within ExecuteOperationAsync,
        /// regardless of success or failure and prior to any exception being thrown.
        /// </summary>
        protected virtual void ExecuteOperationCompleted(OperationContext context)
        {
        }

        public async virtual Task<RetryStatus> ProcessOperationAsync(OperationContext context, TStatusCode statusCode, Exception exception, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(context), context);

            if (CheckIsSuccess(context.State, statusCode, exception))
            {
                return RetryStatus.Succeeded;
            }

            if (RetryPolicy == null)
            {
                return RetryStatus.RetryDisabled;
            }

            if (!CheckIsRetryable(context.State, statusCode, exception))
            {
                return RetryStatus.NonRetryable;
            }

            var canRetry = await RetryPolicy.CheckRetryAndDelayAsync(context.RetryContext, cancellationToken).ConfigureAwait(false);
            if (canRetry)
            {
                return RetryStatus.Retry;
            }

            return RetryStatus.RetryExhausted;
        }

        protected virtual bool CheckIsSuccess(object state, TStatusCode statusCode, Exception exception)
        {
            return exception == null;
        }

        protected virtual bool CheckIsRetryable(object state, TStatusCode statusCode, Exception exception)
        {
            if (statusCode is IRetryable retryableStatusCode)
            {
                return retryableStatusCode.CanRetry;
            }

            if (RetryableExceptionHelper.CheckForRetryableException(exception))
            {
                return true;
            }

            return false;
        }

        protected virtual TStatusCode GetStatusCodeFromResult(object result)
        {
            return result is TStatusCode ? (TStatusCode)result : default;
        }

        protected virtual TStatusCode GetDefaultStatusCode()
        {
            return default;
        }
    }
}

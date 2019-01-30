// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.FaultTolerance;

namespace KodeAid.Net.Http
{
    public class HttpOperationManager : OperationManager
    {
        public Task<OperationStatus> ProcessOperationAsync(OperationContext context, int statusCode, Exception exception, CancellationToken cancellationToken = default)
        {
            return ProcessOperationAsync(context, (object)statusCode, exception, cancellationToken);
        }

        protected override bool CheckIsSuccess(object state, object result, Exception exception)
        {
            return result is int statusCode && statusCode >= 100 && statusCode < 400;
        }

        protected override bool CheckIsRetryable(object state, object result, Exception exception)
        {
            if (result is int statusCode && statusCode >= 500 && statusCode < 600)
            {
                return true;
            }

            return false;
        }
    }
}

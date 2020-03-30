// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using KodeAid.FaultTolerance;

namespace KodeAid.Net.Http
{
    public class HttpOperationManager : OperationManager<int>
    {
        public HttpOperationManager(IRetryPolicy retryPolicy = null)
            : base(retryPolicy)
        {
        }

        protected override bool CheckIsSuccess(object state, int statusCode, Exception exception)
        {
            if (exception != null)
            {
                return false;
            }

            // default of integer suggests we are not using status codes
            if (statusCode == 0)
            {
                return true;
            }

            // we are using status codes, check if it's success
            if (statusCode >= 100 && statusCode < 400)
            {
                return true;
            }

            return false;
        }

        protected override bool CheckIsRetryable(object state, int statusCode, Exception exception)
        {
            if (statusCode >= 500 && statusCode < 600)
            {
                return true;
            }

            // look for a TimeoutException
            if (RetryableExceptionHelper.CheckForRetryableException<TimeoutException>(exception))
            {
                return true;
            }

            // look for a SocketException (root cause) and check if it's retryable
            if (RetryableExceptionHelper.CheckForRetryableSocketException(exception))
            {
                return true;
            }

            // look for an HttpRequestException (high-level) and check if it's retryable
            if (RetryableExceptionHelper.CheckForRetryableHttpRequestException(exception))
            {
                return true;
            }

            return false;
        }
    }
}

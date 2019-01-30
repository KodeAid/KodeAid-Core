// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public interface IRetryPolicy
    {
        int MaxRetryCount { get; }
        TimeSpan MaxRetryDelay { get; }
        Task CheckRetryAndDelayAsync(RetryContext context, CancellationToken cancellationToken = default);
        RetryContext CreateRetryContext();
    }
}

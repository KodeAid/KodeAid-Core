// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public class ExponentialRetry : RetryPolicy
    {
        public int ExponentialPower { get; set; } = 3;

        protected override Task DelayAsync(RetryContext context, CancellationToken cancellationToken = default)
        {
            // delay (to the power of 3): 0s, 1s, 8s, 27s, 64s but if max is at 60s, then: ... 27s, 60s, 60s....
            return Task.Delay(TimeSpan.FromSeconds(Math.Min(Math.Pow(context.RetryCount, ExponentialPower), MaxRetryDelay.TotalSeconds)), cancellationToken);
        }
    }
}

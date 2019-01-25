// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public class ExponentialBackoffRetryPolicy : RetryPolicy
    {
        public int MaxRetryCount { get; set; } = 3;
        public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);
        public int ExponentialPower { get; set; } = 3;

        public override async Task<(bool Retry, RetryContext Context)> RetryDelayAsync(RetryContext context)
        {
            ArgCheck.NotNull(nameof(context), context);

            if (context.RetryCount >= MaxRetryCount)
            {
                return (false, context);
            }

            // delay (to the power of 3): 0s, 1s, 8s, 27s, 64s but if max is at 60s, then: ... 27s, 60s, 60s....
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(Math.Pow(++context.RetryCount, ExponentialPower), MaxRetryDelay.TotalSeconds))).ConfigureAwait(false);

            return (true, context);
        }
    }
}

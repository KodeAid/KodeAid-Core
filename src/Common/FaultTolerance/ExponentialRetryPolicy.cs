// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public class ExponentialRetryPolicy : IRetryPolicy
    {
        public int MaxRetryCount { get; set; } = 3;
        public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);
        public int ExponentialPower { get; set; } = 3;

        public async Task<RetryContext> CheckRetryAsync(RetryContext context)
        {
            ArgCheck.NotNull(nameof(context), context);

            if (context.RetryCount >= MaxRetryCount)
            {
                context.CanRetry = false;
                return context;
            }

            // delay (to the power of 3): 0s, 1s, 8s, 27s, 64s but if max is at 60s, then: ... 27s, 60s, 60s....
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(Math.Pow(++context.RetryCount, ExponentialPower), MaxRetryDelay.TotalSeconds))).ConfigureAwait(false);

            context.CanRetry = true;
            return context;
        }
    }
}

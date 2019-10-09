// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public class ExponentialRetry : RetryPolicy
    {
        /// <summary>
        /// Maximum number of retries, default is three retries.
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// Maximum delay period per retry, default is infinite.
        /// </summary>
        public TimeSpan MaxRetryDelay { get; set; } = Timeout.InfiniteTimeSpan;

        /// <summary>
        /// Delay period base time, default is one second.
        /// </summary>
        public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// delay = BaseDelay * Math.Pow(context.RetryCount, ExponentialPower);
        /// </summary>
        public int ExponentialPower { get; set; } = 3;

        protected override int? GetMaxRetryCount() => MaxRetryCount;

        protected override Task DelayAsync(RetryContext context, CancellationToken cancellationToken = default)
        {
            var delay = TimeSpan.FromSeconds(BaseDelay.TotalSeconds * Math.Pow(context.RetryCount, ExponentialPower));

            if (MaxRetryDelay >= TimeSpan.Zero && delay > MaxRetryDelay)
            {
                delay = MaxRetryDelay;
            }

            // delay (to the power of 3): 0s, 1s, 8s, 27s, 64s but if max is at 60s, then: ... 27s, 60s, 60s....
            return Task.Delay(delay, cancellationToken);
        }
    }
}

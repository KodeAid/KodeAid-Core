// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;

namespace KodeAid
{
    public static class RetryHelper
    {
        public const int DefaultMaxRetryCount = 5;
        public static readonly TimeSpan DefaultMaxRetryDelay = TimeSpan.FromSeconds(60);

        public static async Task ExecuteWithinRetryBlockAsync(this Func<Task> action, int maxRetryCount = DefaultMaxRetryCount, TimeSpan? maxRetryDelay = null)
        {
            await ExecuteWithinRetryBlockAsync(async () => { await action(); return 0; }, maxRetryCount, maxRetryDelay).ConfigureAwait(false);
        }

        public static async Task<T> ExecuteWithinRetryBlockAsync<T>(this Func<Task<T>> action, int maxRetryCount = DefaultMaxRetryCount, TimeSpan? maxRetryDelay = null)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    return await action();
                }
                catch
                {
                    if ((retryCount = await CheckAndDelayForRetryAsync(retryCount, maxRetryCount, maxRetryDelay).ConfigureAwait(false)) == -1)
                        throw;
                }
            }
        }

        public static void ExecuteWithinRetryBlock(this Action action, int maxRetryCount = DefaultMaxRetryCount, TimeSpan? maxRetryDelay = null)
        {
            ExecuteWithinRetryBlock(() => { action(); return 0; }, maxRetryCount, maxRetryDelay);
        }

        public static T ExecuteWithinRetryBlock<T>(this Func<T> action, int maxRetryCount = DefaultMaxRetryCount, TimeSpan? maxRetryDelay = null)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    return action();
                }
                catch
                {
                    if ((retryCount = CheckAndDelayForRetry(retryCount, maxRetryCount, maxRetryDelay)) == -1)
                        throw;
                }
            }
        }

        public static int CheckAndDelayForRetry(int retryCount, int maxRetryCount = DefaultMaxRetryCount, TimeSpan? maxRetryDelay = null)
        {
            return CheckAndDelayForRetryAsync(retryCount, maxRetryCount, maxRetryDelay).GetAwaiter().GetResult();
        }

        public static async Task<int> CheckAndDelayForRetryAsync(int retryCount, int maxRetryCount = DefaultMaxRetryCount, TimeSpan? maxRetryDelay = null)
        {
            if (maxRetryDelay == null)
                maxRetryDelay = DefaultMaxRetryDelay;
            ArgCheck.GreaterThanOrEqualTo(nameof(retryCount), retryCount, 0);
            ArgCheck.GreaterThanOrEqualTo(nameof(maxRetryCount), maxRetryCount, 0);
            ArgCheck.GreaterThanOrEqualTo(nameof(maxRetryDelay), maxRetryDelay.Value, TimeSpan.Zero);
            if (retryCount >= maxRetryCount)
                return -1;
            if (retryCount > 0)
            {
                var delay = TimeSpan.FromMilliseconds((int)Math.Pow(2, retryCount) * 100);
                if (delay > maxRetryDelay)
                    delay = maxRetryDelay.Value;
                await Task.Delay(delay).ConfigureAwait(false);
            }
            return ++retryCount;
        }
    }
}

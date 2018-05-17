// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using KodeAid;

namespace System
{
    public static class FuncExtensions
    {
        public static async Task ExecuteWithinRetryBlock(this Func<Task> action, int maxRetryCount = RetryHelper.DefaultMaxRetryCount, TimeSpan? maxDelayPerRetryInSeconds = null)
        {
            await RetryHelper.ExecuteWithinRetryBlockAsync(action, maxRetryCount, maxDelayPerRetryInSeconds);
        }

        public static async Task<T> ExecuteWithinRetryBlock<T>(this Func<Task<T>> action, int maxRetryCount = RetryHelper.DefaultMaxRetryCount, TimeSpan? maxDelayPerRetryInSeconds = null)
        {
            return await RetryHelper.ExecuteWithinRetryBlockAsync(action, maxRetryCount, maxDelayPerRetryInSeconds);
        }
    }
}

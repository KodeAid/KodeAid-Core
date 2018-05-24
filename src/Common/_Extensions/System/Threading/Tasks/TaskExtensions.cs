// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static Task<T> Await<T>(this Task<T> task, TimeSpan timeout)
        {
            return Await(task, timeout, CancellationToken.None, true);
        }

        public static Task<T> Await<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            return Await(task, Timeout.InfiniteTimeSpan, cancellationToken, true);
        }

        public static Task<T> Await<T>(this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Await(task, timeout, cancellationToken, true);
        }

        public static Task<T> Await<T>(this Task<T> task, TimeSpan timeout, bool continueOnCapturedContext)
        {
            return Await(task, timeout, CancellationToken.None, continueOnCapturedContext);
        }

        public static Task<T> Await<T>(this Task<T> task, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            return Await(task, Timeout.InfiniteTimeSpan, cancellationToken, continueOnCapturedContext);
        }

        public static async Task<T> Await<T>(this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)).ConfigureAwait(continueOnCapturedContext) == task)
                return await task.ConfigureAwait(continueOnCapturedContext);
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException(task);
            throw new TimeoutException();
        }

        public static bool Wait(this Task task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return task.Wait((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static void FireAndForget(this Task task)
        {
            // TODO: not sure this work correctly, there have been some issues with testing, need to confirm this will work every time
            ArgCheck.NotNull(nameof(task), task);
            task.ContinueWith(t =>
            {
                try
                {
                    t.Wait();
                }
                catch { }
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously).ConfigureAwait(false);
        }
    }
}

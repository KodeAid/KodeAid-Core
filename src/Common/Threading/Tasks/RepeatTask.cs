// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Threading.Tasks
{
    public static class RepeatTask
    {
        public static Task Interval(TimeSpan interval, TimeSpan delay, Action action, CancellationToken cancellationToken)
        {
            return Interval(interval, delay, action, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public static Task Interval(TimeSpan interval, TimeSpan delay, Action action, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions)
        {
            return Interval(interval, delay, action, cancellationToken, taskCreationOptions, TaskScheduler.Default);
        }

        public static Task Interval(TimeSpan interval, TimeSpan delay, Action action, CancellationToken cancellationToken, TaskScheduler scheduler)
        {
            return Interval(interval, delay, action, cancellationToken, TaskCreationOptions.LongRunning, scheduler);
        }

        public static Task Interval(TimeSpan interval, TimeSpan delay, Action action, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions, TaskScheduler scheduler)
        {
            ArgCheck.GreaterThan(nameof(interval), interval, TimeSpan.Zero);
            ArgCheck.GreaterThanOrEqualTo(nameof(delay), delay, TimeSpan.Zero);
            ArgCheck.NotNull(nameof(scheduler), scheduler);

            return Task.Factory.StartNew(
                () =>
                {
                    if (cancellationToken.WaitOnCancellationRequested(delay))
                        return;
                    action();
                    while (true)
                    {
                        if (cancellationToken.WaitOnCancellationRequested(interval))
                            break;
                        action();
                    }
                }, cancellationToken, taskCreationOptions, scheduler);
        }
    }
}

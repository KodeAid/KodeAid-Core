// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public class ExplicitRetry : RetryPolicy
    {
        protected override int? GetMaxRetryCount() => RetryDelays.Count;

        public List<TimeSpan> RetryDelays { get; } = new List<TimeSpan>();

        protected override Task DelayAsync(RetryContext context, CancellationToken cancellationToken = default) => Task.Delay(RetryDelays[context.RetryCount], cancellationToken);
    }
}

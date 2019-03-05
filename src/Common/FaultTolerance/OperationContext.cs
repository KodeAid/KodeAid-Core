// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.FaultTolerance
{
    public class OperationContext
    {
        protected internal OperationContext(object state, RetryContext retryContext)
            : this(Guid.NewGuid().ToString("D"), state, retryContext)
        {
        }

        protected internal OperationContext(string operationId, object state, RetryContext retryContext)
        {
            OperationId = operationId;
            State = state;
            RetryContext = retryContext;
        }

        public string OperationId { get; }
        public OperationStatus Status { get; private set; } = OperationStatus.Started;
        public DateTimeOffset StartTime { get; } = DateTimeOffset.Now;
        public DateTimeOffset? CompletedTime { get; private set; }
        public TimeSpan Duration => (CompletedTime ?? DateTimeOffset.Now) - StartTime;
        public object State { get; }
        public RetryContext RetryContext { get; }

        protected internal void SetCompleted(bool succeeded)
        {
            Status = succeeded ? OperationStatus.Succeeded : OperationStatus.Failed;
            CompletedTime = DateTimeOffset.Now;
        }
    }
}

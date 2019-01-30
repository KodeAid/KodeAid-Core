// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



namespace KodeAid.FaultTolerance
{
    public class OperationContext
    {
        protected internal OperationContext(object state, RetryContext retryContext)
        {
            State = state;
            RetryContext = retryContext;
        }

        public object State { get; }
        public RetryContext RetryContext { get; }
    }
}

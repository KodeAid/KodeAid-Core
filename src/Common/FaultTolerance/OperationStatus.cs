// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



namespace KodeAid.FaultTolerance
{
    public enum OperationStatus
    {
        Success,
        RetryDisabled,
        NonRetryable,
        Retry,
        RetryExhausted
    }
}

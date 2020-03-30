// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.FaultTolerance
{
    public class RetryContext
    {
        protected internal RetryContext()
        {
        }

        public int RetryCount { get; internal set; }
    }
}

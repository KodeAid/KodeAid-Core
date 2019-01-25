// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading.Tasks;

namespace KodeAid.FaultTolerance
{
    public interface IRetryPolicy
    {
        Task<RetryContext> CheckRetryAsync(RetryContext context);
    }
}

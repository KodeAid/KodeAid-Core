// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



namespace KodeAid.Data
{
    public interface IOptimisticConcurrency
    {
        // ConcurrencyToken, Timestamp, Version, RowVersion
        object ConcurrencyStamp { get; set; }
    }
}

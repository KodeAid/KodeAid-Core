// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



namespace KodeAid.Data
{
    public interface IOptimisticConcurrency
    {
        // ConcurrencyToken, Timestamp, Version, RowVersion, ETag
        object ConcurrencyStamp { get; set; }
    }
}

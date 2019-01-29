// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.Data;

namespace KodeAid.Storage
{
    public interface IStoreResult : ICreatedTime, IUpdatedTime, IExpiredTime, IOptimisticConcurrency
    {
        string Key { get; set; }
        string Partition { get; set; }
        StoreResultStatus Status { get; set; }
    }
}

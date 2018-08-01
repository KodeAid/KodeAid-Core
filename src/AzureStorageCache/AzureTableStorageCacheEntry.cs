// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace KodeAid.Caching.AzureStorage
{
    public sealed class AzureTableStorageCacheEntry : TableEntity
    {
        public DateTimeOffset LastUpdated { get; set; }
        public DateTimeOffset? Expiration { get; set; }
        public string Value { get; set; }
    }
}

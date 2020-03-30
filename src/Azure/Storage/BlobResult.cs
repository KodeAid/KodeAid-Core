// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using KodeAid.Data;
using KodeAid.Storage;

namespace KodeAid.Azure.Storage
{
    public class BlobResult : IBlobMeta
    {
        public string BlobName { get; set; }
        public string DirectoryRelativeAddress { get; set; }
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public string ETag { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public IDictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

        string IStoreMeta.Partition { get => DirectoryRelativeAddress; set => DirectoryRelativeAddress = value; }
        string IStoreMeta.Key { get => BlobName; set => BlobName = value; }
        DateTimeOffset? ICreatedTime.CreatedAt { get => Created; set => Created = value; }
        DateTimeOffset? IUpdatedTime.UpdatedAt { get => LastModified; set => LastModified = value; }
        DateTimeOffset? IExpiredTime.ExpiresAt { get => Expires; set => Expires = value; }
        object IOptimisticConcurrency.ConcurrencyStamp { get => ETag; set => ETag = (string)value; }
    }
}

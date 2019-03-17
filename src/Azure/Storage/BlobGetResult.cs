// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using KodeAid.Data;
using KodeAid.Storage;

namespace KodeAid.Azure.Storage
{
    public abstract class BlobGetResult : BlobResult, IStoreResult
    {
        public BlobGetResult()
        {
        }

        internal BlobGetResult(BlobGetResult copy)
        {
            if (copy != null)
            {
                BlobName = copy.BlobName;
                ContentEncoding = copy.ContentEncoding;
                ContentType = copy.ContentType;
                Created = copy.Created;
                DirectoryRelativeAddress = copy.DirectoryRelativeAddress;
                ETag = copy.ETag;
                Expires = copy.Expires;
                LastModified = copy.LastModified;
                Status = copy.Status;
            }
        }

        public GetBlobStatus Status { get; set; }

        string IStoreResult.Partition { get => DirectoryRelativeAddress; set => DirectoryRelativeAddress = value; }
        string IStoreResult.Key { get => BlobName; set => BlobName = value; }
        StoreResultStatus IStoreResult.Status { get => (StoreResultStatus)Status; set => Status = (GetBlobStatus)value; }
        DateTimeOffset? ICreatedTime.CreatedAt { get => Created; set => Created = value; }
        DateTimeOffset? IUpdatedTime.UpdatedAt { get => LastModified; set => LastModified = value; }
        DateTimeOffset? IExpiredTime.ExpiresAt { get => Expires; set => Expires = value; }
        object IOptimisticConcurrency.ConcurrencyStamp { get => ETag; set => ETag = (string)value; }
    }
}

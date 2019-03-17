// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


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

        StoreResultStatus IStoreResult.Status { get => (StoreResultStatus)Status; set => Status = (GetBlobStatus)value; }
    }
}

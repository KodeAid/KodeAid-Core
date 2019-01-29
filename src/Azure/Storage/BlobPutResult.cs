// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



namespace KodeAid.Azure.Storage
{
    public class BlobPutResult : BlobResult
    {
        public PutBlobStatus Status { get; set; }
    }
}

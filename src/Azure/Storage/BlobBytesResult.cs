// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.Storage;

namespace KodeAid.Azure.Storage
{
    public class BlobBytesResult : BlobGetResult, IBytesResult
    {
        public BlobBytesResult()
        {
        }

        public BlobBytesResult(byte[] contents)
        {
            Contents = contents;
        }

        internal BlobBytesResult(BlobGetResult copy)
            : base(copy)
        {
        }

        internal BlobBytesResult(BlobGetResult copy, byte[] contents)
            : base(copy)
        {
            Contents = contents;
        }

        public byte[] Contents { get; set; }

        byte[] IBytesResult.Data => Contents;
    }
}

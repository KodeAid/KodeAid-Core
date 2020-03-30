// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.Storage;

namespace KodeAid.Azure.Storage
{
    public class BlobStringResult : BlobGetResult, IStringResult
    {
        public BlobStringResult()
        {
        }

        public BlobStringResult(string contents)
        {
            Contents = contents;
        }

        internal BlobStringResult(BlobGetResult copy)
            : base(copy)
        {
        }

        internal BlobStringResult(BlobGetResult copy, string contents)
            : base(copy)
        {
            Contents = contents;
        }

        public string Contents { get; set; }

        string IStringResult.Value => Contents;
    }
}

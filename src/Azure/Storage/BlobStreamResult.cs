// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using KodeAid.Storage;

namespace KodeAid.Azure.Storage
{
    public class BlobStreamResult : BlobGetResult, IStreamResult, IDisposable
    {
        public BlobStreamResult()
        {
        }

        public BlobStreamResult(Stream contents)
        {
            Contents = contents;
        }

        internal BlobStreamResult(BlobGetResult copy)
            : base(copy)
        {
        }

        internal BlobStreamResult(BlobGetResult copy, Stream contents)
            : base(copy)
        {
            Contents = contents;
        }

        public Stream Contents { get; set; }

        Stream IStreamResult.Stream => Contents;

        public void Dispose()
        {
            Contents?.Dispose();
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using KodeAid.Storage;

namespace KodeAid.Azure.Storage
{
    public class BlobStreamResult : BlobGetResult, IBlobResult, IStreamResult, IDisposable
    {
        public BlobStreamResult()
        {
        }

        public BlobStreamResult(Stream content)
        {
            Content = content;
        }

        internal BlobStreamResult(BlobGetResult copy)
            : base(copy)
        {
        }

        internal BlobStreamResult(BlobGetResult copy, Stream content)
            : base(copy)
        {
            Content = content;
        }

        public Stream Content { get; set; }

        Stream IStreamResult.Stream => Content;

        public void Dispose()
        {
            Content?.Dispose();
        }
    }
}

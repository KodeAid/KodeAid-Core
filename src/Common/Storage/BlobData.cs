// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;

namespace KodeAid.Storage
{
    public class BlobData : IDisposable
    {
        private bool _disposed = false;

        public string Key { get; set; }
        public string Partition { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public object ConcurrencyStamp { get; set; }
        public Stream Content { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public IDictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Content?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}

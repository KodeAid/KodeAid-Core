// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;

namespace KodeAid.Storage
{
    public class StoreResult : IStringResult, IBytesResult, IStreamResult, IDisposable
    {
        private bool _disposed = false;
        private readonly string _value;
        private readonly byte[] _bytes;
        private readonly Stream _stream;

        public StoreResult()
        {
        }

        public StoreResult(Stream stream)
        {
            _stream = stream;
        }

        public StoreResult(byte[] bytes)
        {
            _bytes = bytes;
        }

        public StoreResult(string value)
        {
            _value = value;
        }

        public string Key { get; set; }
        public string Partition { get; set; }
        public StoreResultStatus Status { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public object ConcurrencyStamp { get; set; }

        string IStringResult.Value => _value;
        byte[] IBytesResult.Data => _bytes;
        Stream IStreamResult.Stream => _stream;

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
                    _stream?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}

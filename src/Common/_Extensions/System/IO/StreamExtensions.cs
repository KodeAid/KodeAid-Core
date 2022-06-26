// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;
using KodeAid;

namespace System.IO
{
    public static class StreamExtensions
    {
        private const int _defaultBufferSize = 81920;

        /// <summary>
        /// Reads all bytes from the current stream and writes them to a byte array, using a specified buffer size.
        /// </summary>
        /// <param name="stream">The stream to read all bytes from.</param>
        /// <param name="bufferSize">The size of the buffer. This value must be greater than zero. The default size is 81920.</param>
        /// <param name="disposeOfStream">True to dispose of <paramref name="stream"/> once reading is complete, otherwise false.</param>
        /// <returns>A new byte array.</returns>
        public static byte[] ReadAllBytes(this Stream stream, int bufferSize = _defaultBufferSize, bool disposeOfStream = false)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            using var ms = new MemoryStream();
            stream.CopyTo(ms, bufferSize);
            ms.Flush();
            ms.Position = 0;

            if (disposeOfStream)
            {
                stream.Dispose();
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Asynchronously reads all bytes from the current stream and writes them to a byte array, using a specified buffer size.
        /// </summary>
        /// <param name="stream">The stream to read all bytes from.</param>
        /// <param name="bufferSize">The size of the buffer. This value must be greater than zero. The default size is 81920.</param>
        /// <param name="disposeOfStream">True to dispose of <paramref name="stream"/> once reading is complete, otherwise false.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter is a new byte array.</returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, int bufferSize = _defaultBufferSize, bool disposeOfStream = false, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, bufferSize, cancellationToken).ConfigureAwait(false);
            await ms.FlushAsync(cancellationToken).ConfigureAwait(false);
            ms.Position = 0;

            if (disposeOfStream)
            {
#if NETSTANDARD
                stream.Dispose();
#else
                await stream.DisposeAsync().ConfigureAwait(false);
#endif
            }

            return ms.ToArray();
        }
    }
}

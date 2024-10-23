// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !NET8_0_OR_GREATER

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Serialization.Binary
{
    public class DotNetBinarySerializer : IBinarySerializer, ISerializer, IStreamSerializer, IAsyncStreamSerializer
    {
        public byte[] Serialize(object graph)
        {
            using (var stream = new MemoryStream())
            {
                SerializeToStream(stream, graph);
                return stream.ToArray();
            }
        }

        public object Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return DeserializeFromStream(stream);
            }
        }

        public void SerializeToStream(Stream stream, object graph)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            new BinaryFormatter().Serialize(stream, graph);
        }

        public Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            cancellationToken.ThrowIfCancellationRequested();
            new BinaryFormatter().Serialize(stream, graph);
            return Task.CompletedTask;
        }

        public object DeserializeFromStream(Stream stream)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            return new BinaryFormatter().Deserialize(stream);
        }

        public Task<object> DeserializeFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new BinaryFormatter().Deserialize(stream));
        }

        Type ISerializer.SerializedType => typeof(byte[]);

        object ISerializer.Serialize(object value)
        {
            return Serialize(value);
        }

        object ISerializer<byte[]>.Deserialize(Type type, byte[] data)
        {
            ArgCheck.NotNull(nameof(type), type);

            var result = Deserialize(data);
            if (result != null && result.GetType() != type)
            {
                throw new InvalidCastException();
            }
            return result;
        }

        object ISerializer.Deserialize(Type type, object data)
        {
            ArgCheck.NotNull(nameof(type), type);

            var result = Deserialize((byte[])data);
            if (result != null && result.GetType() != type)
            {
                throw new InvalidCastException();
            }
            return result;
        }

        object IStreamSerializer.DeserializeFromStream(Type type, Stream stream)
        {
            ArgCheck.NotNull(nameof(type), type);
            ArgCheck.NotNull(nameof(stream), stream);

            var result = DeserializeFromStream(stream);
            if (result != null && result.GetType() != type)
            {
                throw new InvalidCastException();
            }
            return result;
        }

        async Task<object> IAsyncStreamSerializer.DeserializeFromStreamAsync(Type type, Stream stream, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(type), type);
            ArgCheck.NotNull(nameof(stream), stream);

            var result = await DeserializeFromStreamAsync(stream, cancellationToken).ConfigureAwait(false);
            if (result != null && result.GetType() != type)
            {
                throw new InvalidCastException();
            }
            return result;
        }
    }
}
#endif

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.Serialization;
using ProtoBuf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.ProtocolBuffers
{
    public class ProtobufSerializer : IBinarySerializer
    {
        public byte[] Serialize(object graph)
        {
            using (var stream = new MemoryStream())
            {
                SerializeToStream(stream, graph);
                return stream.ToArray();
            }
        }

        public object Deserialize(Type type, byte[] data)
        {
            ArgCheck.NotNull(nameof(type), type);

            using (var stream = new MemoryStream(data))
            {
                return DeserializeFromStream(type, stream);
            }
        }

        public void SerializeToStream(Stream stream, object graph)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            Serializer.Serialize(stream, graph);
        }

        public Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            cancellationToken.ThrowIfCancellationRequested();
            Serializer.Serialize(stream, graph);
            return Task.CompletedTask;
        }

        public object DeserializeFromStream(Type type, Stream stream)
        {
            ArgCheck.NotNull(nameof(type), type);
            ArgCheck.NotNull(nameof(stream), stream);

            return Serializer.Deserialize(type, stream);
        }

        public Task<object> DeserializeFromStreamAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(type), type);
            ArgCheck.NotNull(nameof(stream), stream);

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Serializer.Deserialize(type, stream));
        }

        object ISerializer.Serialize(object value)
        {
            return Serialize(value);
        }

        object ISerializer.Deserialize(Type type, object data)
        {
            ArgCheck.NotNull(nameof(type), type);

            return Deserialize(type, (byte[])data);
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Serialization.Binary
{
    public class DotNetBinarySerializer : IBinarySerializer
    {
        public byte[] Serialize(object graph)
        {
            using (var stream = new MemoryStream())
            {
                SerializeToStream(stream, graph);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return DeserializeFromStream<T>(stream);
        }

        public void SerializeToStream(Stream stream, object graph)
        {
            new BinaryFormatter().Serialize(stream, graph);
        }

        public Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            new BinaryFormatter().Serialize(stream, graph);
            return Task.CompletedTask;
        }

        public T DeserializeFromStream<T>(Stream stream)
        {
            return (T)new BinaryFormatter().Deserialize(stream);
        }

        public Task<T> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult((T)new BinaryFormatter().Deserialize(stream));
        }

        object ISerializer.Serialize(object value)
        {
            return Serialize(value);
        }

        T ISerializer.Deserialize<T>(object data)
        {
            return Deserialize<T>((byte[])data);
        }
    }
}

// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Serialization.Binary
{
    public class DotNetBinarySerializer : ISerializer<byte[]>
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
            return Task.Run(() => new BinaryFormatter().Serialize(stream, graph), cancellationToken);
        }

        public T DeserializeFromStream<T>(Stream stream)
        {
            return (T)new BinaryFormatter().Deserialize(stream);
        }

        public Task<T> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => (T)new BinaryFormatter().Deserialize(stream), cancellationToken);
        }

        public void SerializeToFile(string path, object graph, bool overwrite = false)
        {
            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
                SerializeToStream(stream, graph);
        }

        public Task SerializeToFileAsync(string path, object graph, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
                return SerializeToStreamAsync(stream, graph, cancellationToken);
        }

        public T DeserializeFromFile<T>(string path)
        {
            using (var stream = File.OpenRead(path))
                return DeserializeFromStream<T>(stream);
        }

        public Task<T> DeserializeFromFileAsync<T>(string path, CancellationToken cancellationToken = default)
        {
            using (var stream = File.OpenRead(path))
                return DeserializeFromStreamAsync<T>(stream, cancellationToken);
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

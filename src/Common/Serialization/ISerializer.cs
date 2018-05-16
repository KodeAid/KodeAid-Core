// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Serialization.Binary
{
    public interface ISerializer<TSerialized> : ISerializer
    {
        new TSerialized Serialize(object graph);
        T Deserialize<T>(TSerialized data);
    }

    public interface ISerializer
    {
        object Serialize(object graph);
        T Deserialize<T>(object data);
        void SerializeToStream(Stream stream, object graph);
        Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default);
        T DeserializeFromStream<T>(Stream stream);
        Task<T> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default);
        void SerializeToFile(string path, object graph, bool overwrite = false);
        Task SerializeToFileAsync(string path, object graph, bool overwrite = false, CancellationToken cancellationToken = default);
        T DeserializeFromFile<T>(string path);
        Task<T> DeserializeFromFileAsync<T>(string path, CancellationToken cancellationToken = default);
    }
}

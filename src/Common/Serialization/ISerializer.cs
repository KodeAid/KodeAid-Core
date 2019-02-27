// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Serialization
{
    public interface IStringSerializer : ISerializer<string>
    {
    }

    public interface IBinarySerializer : ISerializer<byte[]>
    {
    }

    public interface ISerializer<TSerialized> : ISerializer
    {
        new TSerialized Serialize(object graph);
        object Deserialize(Type type, TSerialized data);
    }

    public interface ISerializer
    {
        object Serialize(object graph);
        object Deserialize(Type type, object data);
        void SerializeToStream(Stream stream, object graph);
        Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default);
        object DeserializeFromStream(Type type, Stream stream);
        Task<object> DeserializeFromStreamAsync(Type type, Stream stream, CancellationToken cancellationToken = default);
    }
}

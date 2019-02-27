// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Serialization
{
    public static class SerializerExtensions
    {
        public static void SerializeToFile(this ISerializer serializer, string path, object graph, bool overwrite = false)
        {
            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                serializer.SerializeToStream(stream, graph);
            }
        }

        public static async Task SerializeToFileAsync(this ISerializer serializer, string path, object graph, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                await serializer.SerializeToStreamAsync(stream, graph, cancellationToken).ConfigureAwait(false);
            }
        }

        public static T Deserialize<T>(this ISerializer serializer, object data)
        {
            return (T)serializer.Deserialize(typeof(T), data);
        }


        public static T DeserializeFromStream<T>(this ISerializer serializer, Stream stream)
        {
            return (T)serializer.DeserializeFromStream(typeof(T), stream);
        }

        public static async Task<T> DeserializeFromStreamAsync<T>(this ISerializer serializer, Stream stream, CancellationToken cancellationToken = default)
        {
            return (T)await serializer.DeserializeFromStreamAsync(typeof(T), stream, cancellationToken).ConfigureAwait(false);
        }

        public static object DeserializeFromFile(this ISerializer serializer, Type type, string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return serializer.DeserializeFromStream(type, stream);
            }
        }

        public static async Task<object> DeserializeFromFileAsync(this ISerializer serializer, Type type, string path, CancellationToken cancellationToken = default)
        {
            using (var stream = File.OpenRead(path))
            {
                return await serializer.DeserializeFromStreamAsync(type, stream, cancellationToken).ConfigureAwait(false);
            }
        }

        public static T DeserializeFromFile<T>(this ISerializer serializer, string path)
        {
            return (T)serializer.DeserializeFromFile(typeof(T), path);
        }

        public static async Task<T> DeserializeFromFileAsync<T>(this ISerializer serializer, string path, CancellationToken cancellationToken = default)
        {
            return (T)await serializer.DeserializeFromFileAsync(typeof(T), path, cancellationToken).ConfigureAwait(false);
        }

        public static T Deserialize<TSerialized, T>(this ISerializer<TSerialized> serializer, TSerialized data)
        {
            return (T)serializer.Deserialize(typeof(T), data);
        }
    }
}

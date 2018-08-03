// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


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
                serializer.SerializeToStream(stream, graph);
        }

        public static async Task SerializeToFileAsync(this ISerializer serializer, string path, object graph, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
                await serializer.SerializeToStreamAsync(stream, graph, cancellationToken).ConfigureAwait(false);
        }

        public static T DeserializeFromFile<T>(this ISerializer serializer, string path)
        {
            using (var stream = File.OpenRead(path))
                return serializer.DeserializeFromStream<T>(stream);
        }

        public static async Task<T> DeserializeFromFileAsync<T>(this ISerializer serializer, string path, CancellationToken cancellationToken = default)
        {
            using (var stream = File.OpenRead(path))
                return await serializer.DeserializeFromStreamAsync<T>(stream, cancellationToken).ConfigureAwait(false);
        }
    }
}

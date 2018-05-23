// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NJsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace KodeAid.Serialization.Json
{
    public class JsonSerializer : ISerializer<string>
    {
        public string Serialize(object graph)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            using (var jw = new JsonTextWriter(sw))
            {
                new NJsonSerializer().Serialize(jw, graph);
                jw.Flush();
                sw.Flush();
            }
            return sb.ToString();
        }

        public T Deserialize<T>(string json)
        {
            using (var sr = new StringReader(json))
            using (var jr = new JsonTextReader(sr))
                return new NJsonSerializer().Deserialize<T>(jr);
        }

        public void SerializeToStream(Stream stream, object graph)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                SerializeToWriter(writer, graph);
                writer.Flush();
            }
        }

        public async Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                SerializeToWriter(writer, graph);
                cancellationToken.ThrowIfCancellationRequested();
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        public T DeserializeFromStream<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return DeserializeFromReader<T>(reader);
            }
        }

        public Task<T> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return Task.FromResult(DeserializeFromReader<T>(reader));
            }
        }

        public void SerializeToFile(string path, object graph, bool overwrite = false)
        {
            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
                SerializeToStream(stream, graph);
        }

        public async Task SerializeToFileAsync(string path, object graph, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
                await SerializeToStreamAsync(stream, graph, cancellationToken).ConfigureAwait(false);
        }

        public T DeserializeFromFile<T>(string path)
        {
            using (var stream = File.OpenRead(path))
                return DeserializeFromStream<T>(stream);
        }

        public async Task<T> DeserializeFromFileAsync<T>(string path, CancellationToken cancellationToken = default)
        {
            using (var stream = File.OpenRead(path))
                return await DeserializeFromStreamAsync<T>(stream, cancellationToken).ConfigureAwait(false);
        }

        public void SerializeToWriter(TextWriter writer, object graph)
        {
            using (var jw = new JsonTextWriter(writer))
            {
                new NJsonSerializer().Serialize(jw, graph);
                jw.Flush();
            }
        }

        public T DeserializeFromReader<T>(TextReader reader)
        {
            using (var jr = new JsonTextReader(reader))
            {
                return new NJsonSerializer().Deserialize<T>(jr);
            }
        }

        object ISerializer.Serialize(object value)
        {
            return Serialize(value);
        }

        T ISerializer.Deserialize<T>(object data)
        {
            return Deserialize<T>((string)data);
        }
    }
}

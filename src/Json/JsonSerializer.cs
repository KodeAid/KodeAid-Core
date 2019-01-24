// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.Serialization;
using Newtonsoft.Json;

namespace KodeAid.Json
{
    public class JsonSerializer : IStringSerializer
    {
        public JsonSerializerSettings Settings { get; set; }

        public string Serialize(object graph)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            using (var jw = new JsonTextWriter(sw))
            {
                CreateJsonSerializer().Serialize(jw, graph);
                jw.Flush();
                sw.Flush();
            }
            return sb.ToString();
        }

        public T Deserialize<T>(string json)
        {
            using (var sr = new StringReader(json))
            using (var jr = new JsonTextReader(sr))
                return CreateJsonSerializer().Deserialize<T>(jr);
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

        public void SerializeToWriter(TextWriter writer, object graph)
        {
            using (var jw = new JsonTextWriter(writer))
            {
                CreateJsonSerializer().Serialize(jw, graph);
                jw.Flush();
            }
        }

        public T DeserializeFromReader<T>(TextReader reader)
        {
            using (var jr = new JsonTextReader(reader))
            {
                return CreateJsonSerializer().Deserialize<T>(jr);
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

        private Newtonsoft.Json.JsonSerializer CreateJsonSerializer()
        {
            return Newtonsoft.Json.JsonSerializer.CreateDefault(Settings);
        }
    }
}

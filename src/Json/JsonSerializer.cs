// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.Serialization;
using Newtonsoft.Json;

namespace KodeAid.Json
{
    public class JsonSerializer : IStringSerializer, ISerializer, IStreamSerializer, IAsyncStreamSerializer
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

        public object Deserialize(Type type, string json)
        {
            ArgCheck.NotNull(nameof(type), type);

            using (var sr = new StringReader(json))
            using (var jr = new JsonTextReader(sr))
            {
                return CreateJsonSerializer().Deserialize(jr, type);
            }
        }

        public void SerializeToStream(Stream stream, object graph)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                SerializeToWriter(writer, graph);
                writer.Flush();
            }
        }

        public async Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            cancellationToken.ThrowIfCancellationRequested();
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                SerializeToWriter(writer, graph);
                cancellationToken.ThrowIfCancellationRequested();
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        public object DeserializeFromStream(Type type, Stream stream)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return DeserializeFromReader(type, reader);
            }
        }

        public Task<object> DeserializeFromStreamAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(stream), stream);

            cancellationToken.ThrowIfCancellationRequested();
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return Task.FromResult(DeserializeFromReader(type, reader));
            }
        }

        public void SerializeToWriter(TextWriter writer, object graph)
        {
            ArgCheck.NotNull(nameof(writer), writer);

            using (var jw = new JsonTextWriter(writer))
            {
                CreateJsonSerializer().Serialize(jw, graph);
                jw.Flush();
            }
        }

        public object DeserializeFromReader(Type type, TextReader reader)
        {
            ArgCheck.NotNull(nameof(reader), reader);

            using (var jr = new JsonTextReader(reader))
            {
                return CreateJsonSerializer().Deserialize(jr, type);
            }
        }

        Type ISerializer.SerializedType => typeof(string);

        object ISerializer.Serialize(object value)
        {
            return Serialize(value);
        }

        object ISerializer.Deserialize(Type type, object data)
        {
            return Deserialize(type, (string)data);
        }

        private Newtonsoft.Json.JsonSerializer CreateJsonSerializer()
        {
            return Newtonsoft.Json.JsonSerializer.CreateDefault(Settings);
        }
    }
}

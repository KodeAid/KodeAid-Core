//// Copyright (c) Kris Penner. All rights reserved.
//// Licensed under the MIT License. See LICENSE in the project root for license information.


//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;

//namespace KodeAid.Serialization.Binary
//{
//    public abstract class BaseSerializer<TSerialized> : ISerializer<TSerialized>
//    {
//        protected abstract Stream

//        public abstract void SerializeToStream(Stream stream, object graph);

//        public abstract Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default);

//        public abstract T DeserializeFromStream<T>(Stream stream);

//        public abstract Task<T> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default);

//        public TSerialized Serialize(object graph)
//        {
//            using (var stream = new MemoryStream())
//            {
//                SerializeToStream(stream, graph);
//                return stream.ToArray();
//            }
//        }

//        public T Deserialize<T>(TSerialized data)
//        {
//            using (var stream = new MemoryStream())
//                return DeserializeFromStream<T>(stream);
//        }

//        public void SerializeToFile(string path, object graph, bool overwrite = false)
//        {
//            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
//                SerializeToStream(stream, graph);
//        }

//        public Task SerializeToFileAsync(string path, object graph, bool overwrite = false, CancellationToken cancellationToken = default)
//        {
//            using (var stream = File.Open(path, overwrite ? FileMode.Create : FileMode.CreateNew))
//                return SerializeToStreamAsync(stream, graph, cancellationToken);
//        }

//        public T DeserializeFromFile<T>(string path)
//        {
//            using (var stream = File.OpenRead(path))
//                return DeserializeFromStream<T>(stream);
//        }

//        public Task<T> DeserializeFromFileAsync<T>(string path, CancellationToken cancellationToken = default)
//        {
//            using (var stream = File.OpenRead(path))
//                return DeserializeFromStreamAsync<T>(stream, cancellationToken);
//        }

//        object ISerializer.Serialize(object value)
//        {
//            return Serialize(value);
//        }

//        T ISerializer.Deserialize<T>(object data)
//        {
//            return Deserialize<T>((TSerialized)data);
//        }
//    }
//}

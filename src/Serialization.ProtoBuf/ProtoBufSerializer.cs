// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.IO;
using ProtoBuf;

namespace KodeAid.Serialization.ProtoBuf
{
    public static class ProtoBufSerializer
    {
        public static byte[] Serialize(object value)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return Serializer.Deserialize<T>(stream);
        }

        public static void SerializeToFile(string fileName, object value)
        {
            using (var stream = File.Open(fileName, FileMode.Create))
                Serializer.Serialize(stream, value);
        }

        public static T DeserializeFromFile<T>(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open))
                return Serializer.Deserialize<T>(stream);
        }
    }
}

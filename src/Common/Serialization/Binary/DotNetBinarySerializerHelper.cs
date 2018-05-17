// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KodeAid.Serialization.Binary
{
    public class DotNetBinarySerializerHelper
    {
        public static byte[] Serialize(object value)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return (T)new BinaryFormatter().Deserialize(stream);
        }

        public static void Serialize(string fileName, object value)
        {
            using (var stream = File.Open(fileName, FileMode.Create))
                new BinaryFormatter().Serialize(stream, value);
        }

        public static T Deserialize<T>(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open))
                return (T)new BinaryFormatter().Deserialize(stream);
        }
    }
}

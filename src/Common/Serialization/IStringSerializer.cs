// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;

namespace KodeAid.Serialization
{
    public interface IStringSerializer : ISerializer<string>
    {
        void SerializeToWriter(TextWriter writer, object graph);
        object DeserializeFromReader(Type type, TextReader reader);
    }
}

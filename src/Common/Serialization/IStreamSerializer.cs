// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;

namespace KodeAid.Serialization
{
    public interface IStreamSerializer
    {
        void SerializeToStream(Stream stream, object graph);
        object DeserializeFromStream(Type type, Stream stream);
    }
}

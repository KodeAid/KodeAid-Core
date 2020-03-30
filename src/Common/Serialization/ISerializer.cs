// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Serialization
{
    public interface ISerializer<TSerialized>
    {
        TSerialized Serialize(object graph);
        object Deserialize(Type type, TSerialized serialized);
    }

    public interface ISerializer
    {
        Type SerializedType { get; }
        object Serialize(object graph);
        object Deserialize(Type type, object serialized);
    }
}

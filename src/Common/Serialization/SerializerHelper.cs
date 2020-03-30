// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Serialization
{
    public static class SerializerHelper
    {
        public static ISerializer WrapSerializer<TSerialized>(ISerializer<TSerialized> serializer)
        {
            return new SerializerWrapperImp<TSerialized>(serializer);
        }

        private sealed class SerializerWrapperImp<TSerialized> : ISerializer
        {
            private readonly ISerializer<TSerialized> _serializer;

            public SerializerWrapperImp(ISerializer<TSerialized> serializer)
            {
                ArgCheck.NotNull(nameof(serializer), serializer);

                _serializer = serializer;
            }

            public Type SerializedType => _serializer.GetType().GenericTypeArguments[0];

            public object Serialize(object graph)
            {
                return _serializer.Serialize(graph);
            }

            public object Deserialize(Type type, object serialized)
            {
                return _serializer.Deserialize(type, (TSerialized)serialized);
            }
        }
    }
}

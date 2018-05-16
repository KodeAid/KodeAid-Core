// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KodeAid.Serialization.Json.Converters
{
    public class SingleValueArrayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
                return serializer.Deserialize(reader, objectType);

            // create array with length one for the single instance
            if (objectType.IsArray)
            {
                var array = Array.CreateInstance(objectType.GetElementType(), 1);
                array.SetValue(serializer.Deserialize(reader, objectType.GetElementType()), 0);
                return array;
            }

            // create generic collection for the single instance
            if (objectType.IsGenericType)
            {
                if (objectType.GetGenericArguments().Length == 1)
                {
                    var elementType = objectType.GetGenericArguments()[0];
                    if (typeof(ICollection<>).MakeGenericType(elementType).IsAssignableFrom(objectType))
                    {
                        var collection = Activator.CreateInstance(objectType);
                        collection.GetType().GetMethod("Add", new[] { elementType }).Invoke(collection, new[] { serializer.Deserialize(reader, elementType) });
                        return collection;
                    }
                }
            }

            // shouldn't get here, this will most likely fail on the assignment after we return of this method
            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}

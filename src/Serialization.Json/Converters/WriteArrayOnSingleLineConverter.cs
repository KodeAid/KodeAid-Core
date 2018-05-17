// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections;
using Newtonsoft.Json;

namespace KodeAid.Serialization.Json.Converters
{
    public class WriteArrayOnSingleLineConverter : JsonConverter
    {
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(string))
                return false;
            if (typeof(IEnumerable).IsAssignableFrom(objectType))
                return true;
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotSupportedException($"{nameof(WriteArrayOnSingleLineConverter)} is only for writing JSON, it cannot read JSON.");
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.None));
        }
    }
}

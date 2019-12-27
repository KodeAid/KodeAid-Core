// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace KodeAid.Json.Converters
{
    public class StringArrayEnumFlagsConverter : StringEnumConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var type = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return type.IsEnum && type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 1;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            // if value is not an array (we want string[]) then use base implementation
            if (reader.TokenType != JsonToken.StartArray)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            // convert JSON array to comma separated string and use base implementation to convert to enum
            var value = serializer.Deserialize(reader);
            using var sr = new StringReader($@"""{string.Join(",", ((JArray)value).Values<string>())}""");
            using var jr = new JsonTextReader(sr);
            jr.Read();
            return base.ReadJson(jr, Nullable.GetUnderlyingType(objectType) ?? objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var type = value.GetType();
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (Equals(value, Activator.CreateInstance(type)))
            {
                writer.WriteStartArray();
                writer.WriteEndArray();
                return;
            }

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            using (var jw = new JsonTextWriter(sw))
            {
                base.WriteJson(jw, value, serializer);
                jw.Flush();
                sw.Flush();
            }

            value = sb.ToString().Trim('\'', '"').Split(',').Select(s => s.Trim()).ToArray();
            serializer.Serialize(writer, value);
        }
    }
}

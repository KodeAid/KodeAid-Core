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
    /// <summary>
    /// Converts an <see cref="Enum"/> to and from its name string value and uses an array of strings for any with the <see cref="FlagsAttribute"/> defined instead of the default comma separated list.
    /// </summary>
    public class StringArrayEnumConverter : StringEnumConverter
    {
        public StringArrayEnumConverter()
        {
            AllowIntegerValues = true;
        }

        public override bool CanConvert(Type objectType)
        {
            if (base.CanConvert(objectType))
            {
                return true;
            }

            var type = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return type.IsEnum && type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 1;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var type = Nullable.GetUnderlyingType(objectType) ?? objectType;

            // we only care about enums with the flags attribute
            if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
            {
                // read default enum
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            // value is not string[], assume underlying enum type (typically int)
            if (reader.TokenType != JsonToken.StartArray)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            var value = serializer.Deserialize(reader);
            using var sr = new StringReader("\"" + string.Join(", ", ((JArray)value).Values<string>()) + "\"");
            using var jr = new JsonTextReader(sr);
            jr.Read();
            return base.ReadJson(jr, type, Activator.CreateInstance(type), serializer);
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

            if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
            {
                // write default enum
                base.WriteJson(writer, value, serializer);
            }

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

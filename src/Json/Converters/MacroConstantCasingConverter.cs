// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace KodeAid.Json.Converters
{
    public class MacroConstantCasingConverter : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            return type == typeof(string) || typeof(IEnumerable<string>).IsAssignableFrom(type) || (Nullable.GetUnderlyingType(type) ?? type).IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type type, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            var flags = type.IsEnum && type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 1;
            var array = flags || typeof(IEnumerable<string>).IsAssignableFrom(type);
            var value = serializer.Deserialize(reader, array ? typeof(string[]) : typeof(string));
            if (value == null)
            {
                return null;
            }

            if (flags)
            {
                return Enum.Parse(type, string.Join(",", ((string[])value).Select(s => s.Replace("_", ""))), true);
            }

            if (type.IsEnum)
            {
                return Enum.Parse(type, ((string)value).Replace("_", ""), true);
            }

            if (array)
            {
                return ((string[])value).Select(s => s.ToMacroConstantCase()).ToList();
            }

            return ((string)value).ToMacroConstantCase();
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var type = value.GetType();
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsEnum)
            {
                if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 1)
                {
                    value = value.ToString().Split(',').Select(s => s.Trim()).ToArray();
                }
                else
                {
                    value = value.ToString();
                }

                type = value.GetType();
            }

            if (typeof(IEnumerable<string>).IsAssignableFrom(type))
            {
                value = ((IEnumerable<string>)value).Select(v => v?.ToString().ToMacroConstantCase()).ToArray();
            }
            else
            {
                value = value?.ToString().ToMacroConstantCase();
            }

            serializer.Serialize(writer, value);
        }
    }
}

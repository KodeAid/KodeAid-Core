// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Linq;
using Newtonsoft.Json;

namespace KodeAid.Json.Converters
{
    public class PascalCasingConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // TODO: replace all the below with a regex

            var hasInvalid = value.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '_' && c != '.');
            if (hasInvalid)
            {
                return value;
            }

            var hasLetters = value.Any(c => char.IsLetter(c));
            if (!hasLetters)
            {
                return value;
            }

            // if it has any lower case letters then assume it is already trying to do some form of camel/pascal casing and take this into account
            var hasLowerCase = value.Any(c => char.IsLetter(c) && char.IsLower(c));

            return string.Join("", value.Split(' ', '-', '_', '.').Where(s => s.Length > 0).Select(s => hasLowerCase && s.Length <= 2 ? s : (char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant())));
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}

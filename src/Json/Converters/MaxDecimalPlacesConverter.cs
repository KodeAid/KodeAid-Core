// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using Newtonsoft.Json;

namespace KodeAid.Serialization.Json.Converters
{
    public class MaxDecimalPlacesConverter : JsonConverter
    {
        public MaxDecimalPlacesConverter()
            : this(2)
        {
        }

        public MaxDecimalPlacesConverter(int maxDecimalPlaces)
        {
            ArgCheck.GreaterThanOrEqualTo(nameof(maxDecimalPlaces), maxDecimalPlaces, -1);
            MaxDecimalPlaces = maxDecimalPlaces;
        }

        public int MaxDecimalPlaces { get; set; }

        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            objectType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return objectType == typeof(decimal) || objectType == typeof(double) || objectType == typeof(float);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, value);
                return;
            }

            if (value is decimal d)
            {
                serializer.Serialize(writer, decimal.Round(d, MaxDecimalPlaces, MidpointRounding.AwayFromZero));
                return;
            }

            if (value is double d1)
            {
                serializer.Serialize(writer, Math.Round(d1, MaxDecimalPlaces, MidpointRounding.AwayFromZero));
                return;
            }

            if (value is float f)
            {
                serializer.Serialize(writer, Math.Round(f, MaxDecimalPlaces, MidpointRounding.AwayFromZero));
                return;
            }

            throw new InvalidCastException($"{GetType().Name} failed to cast value to decimal, double or float; value is of type {value.GetType().FullName}.");
        }
    }
}

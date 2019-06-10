// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Globalization;
using Newtonsoft.Json;

namespace KodeAid.Json.Converters
{
    public class MaxDecimalPlacesConverter : JsonConverter
    {
        public const int InifinteDecimalPlaces = -1;

        public MaxDecimalPlacesConverter()
            : this(2)
        {
        }

        public MaxDecimalPlacesConverter(int maxDecimalPlaces)
            : this(maxDecimalPlaces, true)
        {
        }

        public MaxDecimalPlacesConverter(int maxDecimalPlaces, bool forceDecimalPlaces)
        {
            ArgCheck.GreaterThanOrEqualTo(nameof(maxDecimalPlaces), maxDecimalPlaces, InifinteDecimalPlaces);

            MaxDecimalPlaces = maxDecimalPlaces;
            ForceDecimalPlaces = forceDecimalPlaces;
        }

        public int MaxDecimalPlaces { get; set; }

        public bool ForceDecimalPlaces { get; set; }

        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            objectType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return objectType == typeof(decimal) || objectType == typeof(double) || objectType == typeof(float);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null || MaxDecimalPlaces == InifinteDecimalPlaces)
            {
                serializer.Serialize(writer, value);
                return;
            }

            if (value is decimal m)
            {
                m = decimal.Round(m, MaxDecimalPlaces, MidpointRounding.AwayFromZero);
                if (ForceDecimalPlaces)
                {
                    writer.WriteRawValue(m.ToString($"F{MaxDecimalPlaces}", CultureInfo.InvariantCulture));
                }
                else
                {
                    serializer.Serialize(writer, m);
                }
                return;
            }

            if (value is double d)
            {
                d = Math.Round(d, MaxDecimalPlaces, MidpointRounding.AwayFromZero);
                if (ForceDecimalPlaces)
                {
                    writer.WriteRawValue(d.ToString($"F{MaxDecimalPlaces}", CultureInfo.InvariantCulture));
                }
                else
                {
                    serializer.Serialize(writer, d);
                }
                return;
            }

            if (value is float f)
            {
                d = Math.Round(f, MaxDecimalPlaces, MidpointRounding.AwayFromZero);
                if (ForceDecimalPlaces)
                {
                    writer.WriteRawValue(d.ToString($"F{MaxDecimalPlaces}", CultureInfo.InvariantCulture));
                }
                else
                {
                    serializer.Serialize(writer, d);
                }
                return;
            }

            throw new InvalidCastException($"{GetType().Name} failed to cast value to decimal, double or float; value is of type {value.GetType().FullName}.");
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Globalization;
using KodeAid.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KodeAid.Json.Converters
{
    /// <summary>
    /// Writes a <see cref="DateTime"/> to the specified format and reads from mulitple formats.
    /// </summary>
    public class DateTimeConverter : DateTimeConverterBase
    {
        private const string _defaultWriteDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fffffffK";

        private string _writeDateTimeFormat;
        private CultureInfo _culture;

        public bool AllowDateTime { get; set; } = true;

        public bool AllowDateTimeOffset { get; set; } = true;

        /// <summary>
        /// Gets or sets the culture used when converting a date to and from JSON.
        /// </summary>
        /// <value>The culture used when converting a date to and from JSON.</value>
        public CultureInfo Culture
        {
            get => _culture ?? CultureInfo.CurrentCulture;
            set => _culture = value;
        }

        /// <summary>
        /// Gets or sets the date time styles used when converting a date to and from JSON.
        /// </summary>
        /// <value>The date time styles used when converting a date to and from JSON.</value>
        public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.RoundtripKind;

        /// <summary>
        /// Gets or sets accepted date time formats used when reading a date from JSON.
        /// To clear, set to an empty array. To reset to default, set to null.
        /// </summary>
        /// <value>Accepted date time formats used when reading a date from JSON.</value>
        public string[] ReadDateTimeFormats { get; set; }

        /// <summary>
        /// Gets or sets the date time format used when writing a date to JSON.
        /// To reset to default, set to null.
        /// Default is "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK".
        /// </summary>
        /// <value>The date time format used when writing a date to JSON.</value>
        public string WriteDateTimeFormat
        {
            get => _writeDateTimeFormat ?? _defaultWriteDateTimeFormat;
            set => _writeDateTimeFormat = value;
        }

        public override bool CanConvert(Type objectType)
        {
            if (!base.CanConvert(objectType))
            {
                return false;
            }

            if (AllowDateTime && (objectType == typeof(DateTime) || objectType == typeof(DateTime?)))
            {
                return true;
            }

            if (AllowDateTimeOffset && (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            string text;

            if (value is DateTime dateTime)
            {
                if (DateTimeStyles.HasFlag(DateTimeStyles.AdjustToUniversal))
                {
                    dateTime = dateTime.ToUniversalTime();
                }

                text = dateTime.ToString(WriteDateTimeFormat, Culture);
            }
            else if (value is DateTimeOffset dateTimeOffset)
            {
                if (DateTimeStyles.HasFlag(DateTimeStyles.AdjustToUniversal))
                {
                    dateTimeOffset = dateTimeOffset.ToUniversalTime();
                }

                text = dateTimeOffset.ToString(WriteDateTimeFormat, Culture);
            }
            else
            {
                var expectedTypes = $"{(AllowDateTime ? nameof(DateTime) : null)}{((AllowDateTime && AllowDateTimeOffset) ? " or " : null)}{(AllowDateTimeOffset ? nameof(DateTimeOffset) : null)}";
                throw new JsonSerializationException($"Unexpected value when converting date. Expected {expectedTypes}, got {value.GetType().FullName}.");
            }

            writer.WriteValue(text);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var nullable = ReflectionHelper.IsNullableType(objectType);

            if (reader.TokenType == JsonToken.Null)
            {
                if (!nullable)
                {
                    throw new JsonSerializationException($"Cannot convert null value to {objectType.FullName}.");
                }

                return null;
            }

            var t = (nullable)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            if (reader.TokenType == JsonToken.Date)
            {
                if (t == typeof(DateTimeOffset))
                {
                    return (reader.Value is DateTimeOffset) ? reader.Value : new DateTimeOffset((DateTime)reader.Value);
                }

                // converter is expected to return a DateTime
                if (reader.Value is DateTimeOffset offset)
                {
                    return offset.DateTime;
                }

                return reader.Value;
            }

            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonSerializationException($"Unexpected token parsing date. Expected String, got {reader.TokenType}.");
            }

            var dateText = reader.Value.ToString();

            if (string.IsNullOrEmpty(dateText) && nullable)
            {
                return null;
            }

            if (t == typeof(DateTimeOffset))
            {
                if (ReadDateTimeFormats != null && ReadDateTimeFormats.Length > 0)
                {
                    return DateTimeOffset.ParseExact(dateText, ReadDateTimeFormats, Culture, DateTimeStyles);
                }
                else
                {
                    return DateTimeOffset.Parse(dateText, Culture, DateTimeStyles);
                }
            }

            if (ReadDateTimeFormats != null && ReadDateTimeFormats.Length > 0)
            {
                return DateTime.ParseExact(dateText, ReadDateTimeFormats, Culture, DateTimeStyles);
            }
            else
            {
                return DateTime.Parse(dateText, Culture, DateTimeStyles);
            }
        }
    }
}

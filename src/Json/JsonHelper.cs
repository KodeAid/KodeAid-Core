// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Globalization;
using Newtonsoft.Json;

namespace KodeAid.Json
{
    /// <summary>
    /// Helpers for configuring the default <see cref="JsonSerializerSettings"/> used by Json.NET,
    /// namely parsing floating point numbers as <see cref="decimal"/>, leaving date-like strings as strings,
    /// writing unformatted JSON and including null values.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Sets <see cref="JsonConvert.DefaultSettings"/> so that all serializers created via
        /// <see cref="JsonConvert"/> and <see cref="JsonSerializer.CreateDefault()"/> parse numbers as
        /// <see cref="decimal"/>, do not parse or format dates, write unformatted JSON and include null values.
        /// </summary>
        /// <param name="configure">An optional callback to further configure the settings.</param>
        public static void SetDefaultSettings(Action<JsonSerializerSettings>? configure = null)
        {
            JsonConvert.DefaultSettings = () => CreateDefaultSettings(configure);
        }

        /// <summary>
        /// Creates a new <see cref="JsonSerializerSettings"/> which parses numbers as <see cref="decimal"/>,
        /// does not parse or format dates, writes unformatted JSON and includes null values.
        /// </summary>
        /// <param name="configure">An optional callback to further configure the settings.</param>
        /// <returns>The new settings.</returns>
        public static JsonSerializerSettings CreateDefaultSettings(Action<JsonSerializerSettings>? configure = null)
        {
            var settings = ApplyDefaults(new JsonSerializerSettings());
            configure?.Invoke(settings);
            return settings;
        }

        /// <summary>
        /// Applies the default settings to an existing <see cref="JsonSerializerSettings"/>,
        /// parsing numbers as <see cref="decimal"/>, neither parsing nor formatting dates,
        /// writing unformatted JSON and including null values.
        /// </summary>
        /// <param name="settings">The settings to apply the defaults to.</param>
        /// <returns>The same <paramref name="settings"/> instance for chaining.</returns>
        public static JsonSerializerSettings ApplyDefaults(JsonSerializerSettings settings)
        {
            ArgCheck.NotNull(nameof(settings), settings);

            // numbers: read floating point values as decimal instead of double,
            // and write them as-is rather than as symbols such as "NaN" or "Infinity".
            settings.FloatParseHandling = FloatParseHandling.Decimal;
            settings.FloatFormatHandling = FloatFormatHandling.String;

            // dates: leave date-like strings as strings, and write DateTime/DateTimeOffset values
            // using the ISO 8601 round-trip format without any time zone conversion.
            settings.DateParseHandling = DateParseHandling.None;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

            // output: no indentation or extra whitespace, and both null and default values
            // are written out rather than omitted.
            settings.Formatting = Formatting.None;
            settings.NullValueHandling = NullValueHandling.Include;
            settings.DefaultValueHandling = DefaultValueHandling.Include;

            // types: never honor a "$type" property on read, it allows arbitrary types to be constructed,
            // and skip the metadata read-ahead as there is no "$type"/"$ref" to look for.
            settings.TypeNameHandling = TypeNameHandling.None;
            settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;

            // culture: always parse and format using the invariant culture, otherwise the
            // decimal separator and date formats would vary by the current thread's culture.
            settings.Culture = CultureInfo.InvariantCulture;

            return settings;
        }
    }
}

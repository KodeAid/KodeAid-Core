// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Globalization;

namespace KodeAid.Json.Converters
{
    /// <summary>
    /// Writes a <see cref="DateTime"/> to the specified format and reads from mulitple formats.
    /// All times read are assumed to be in UTC if no offset is specified, all dates written are adjusted to UTC.
    /// The default write format is "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK".
    /// The default read formats are "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK", "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK", "yyyy'-'MM'-'dd'T'HH':'mm':'ssK", "yyyy'-'MM'-'dd'T'HH':'mmK".
    /// </summary>
    public class UtcDateTimeConverter : DateTimeConverter
    {
        private const string _defaultWriteDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fffffffK";

        private static readonly string[] _defaultReadDateTimeFormats = new[]
        {
            "yyyy-MM-dd'T'HH:mm:ss.fffffffK",
            "yyyy-MM-dd'T'HH:mm:ss.fffK",
            "yyyy-MM-dd'T'HH:mm:ssK",
            "yyyy-MM-dd'T'HH:mmK",
        };

        public UtcDateTimeConverter()
        {
            DateTimeStyles =  DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind;
            WriteDateTimeFormat = _defaultWriteDateTimeFormat;
            ReadDateTimeFormats = _defaultReadDateTimeFormats;
        }
    }
}

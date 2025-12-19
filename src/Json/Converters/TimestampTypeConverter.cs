// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace KodeAid.Json.Converters
{
    /// <summary>
    /// Converts any DateTimeOffset to/from a date and time with offset JSON string formatted value.
    /// Default format of "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK".
    /// </summary>
    public class TimestampTypeConverter : DateTimeConverter
    {
        public TimestampTypeConverter()
            : this("yyyy-MM-dd'T'HH:mm:ss.fffK")
        {
        }

        public TimestampTypeConverter(string format)
        {
            ArgCheck.NotNull(nameof(format), format);

            AllowDateTime = false;
            AllowDateTimeOffset = true;
            WriteDateTimeFormat = format;
            ReadDateTimeFormats = new[] { format };
        }
    }
}

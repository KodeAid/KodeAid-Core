// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace KodeAid.Json.Converters
{
    /// <summary>
    /// Converts any DateTime to/from a date only JSON string formatted value.
    /// Default format of "yyyy'-'MM'-'dd'".
    /// </summary>
    public class DateTimeTypeAsDateConverter : DateTimeConverter
    {
        public DateTimeTypeAsDateConverter()
            : this("yyyy-MM-dd")
        {
        }

        public DateTimeTypeAsDateConverter(string format)
        {
            ArgCheck.NotNull(nameof(format), format);

            AllowDateTime = true;
            AllowDateTimeOffset = false;
            WriteDateTimeFormat = format;
            ReadDateTimeFormats = new[] { format };
        }
    }
}

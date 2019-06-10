// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.Json.Converters
{
    public class DateConverter : DateTimeConverter
    {
        public DateConverter()
            : this("yyyy-MM-dd")
        {
        }

        public DateConverter(string format)
        {
            ArgCheck.NotNull(nameof(format), format);

            WriteDateTimeFormat = format;
            ReadDateTimeFormats = new[] { format };
        }
    }
}

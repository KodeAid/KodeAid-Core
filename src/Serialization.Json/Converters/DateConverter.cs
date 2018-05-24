// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Newtonsoft.Json.Converters;

namespace KodeAid.Serialization.Json.Converters
{
    public class DateConverter : IsoDateTimeConverter
    {
        public DateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}

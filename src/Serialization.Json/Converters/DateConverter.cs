// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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

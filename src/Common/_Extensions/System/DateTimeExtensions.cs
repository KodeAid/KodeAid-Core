// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTimeMilliseconds(this long epochMilliseconds)
        {
            ArgCheck.GreaterThanOrEqualTo(nameof(epochMilliseconds), epochMilliseconds, 0);
            return Epoch.AddMilliseconds(epochMilliseconds);
        }

        public static DateTime FromUnixTimeSeconds(this long epochMilliseconds)
        {
            ArgCheck.GreaterThanOrEqualTo(nameof(epochMilliseconds), epochMilliseconds, 0);
            return Epoch.AddSeconds(epochMilliseconds);
        }

        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(Epoch).TotalMilliseconds;
        }

        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(Epoch).TotalSeconds;
        }

        public static string ToFriendlyString(this DateTime date, out bool isFriendlyName, string formatThisYear = "ddd, dd MMM", string formatOtherYear = "dd MMM yyyy", int maxMonthNameLength = 5)
        {
            date = date.Date;
            var today = DateTime.Today;

            isFriendlyName = true;
            if (date == today)
                return "Today";
            if (date == today.AddDays(-1))
                return "Yesterday";
            if (date == today.AddDays(1))
                return "Tomorrow";
            isFriendlyName = false;

            var format = formatOtherYear;
            if (date.Year == today.Year ||  // this year
              (date.Year == today.Year - 1 && today.Month == 1 && date.Month == 12) ||  // last year but within one month of today
              (date.Year == today.Year + 1 && today.Month == 12 && date.Month == 1))  // next year but within one month of today
                format = formatThisYear;

            var str = date.ToString(format);
            if (format.Contains("MMMM"))
            {
                var monthName = date.ToString("MMMM");
                if (monthName.Length > maxMonthNameLength)
                    str.Replace(monthName, date.ToString("MMM"));
            }
            return str;
        }
    }
}

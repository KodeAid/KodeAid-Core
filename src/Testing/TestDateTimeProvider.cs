// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Testing
{
    public class TestDateTimeProvider : IDateTimeProvider
    {
        private DateTimeOffset? _dateTime;

        public TestDateTimeProvider()
            : this(null, null, null)
        {
        }

        public TestDateTimeProvider(DateTimeOffset dateTime)
            : this(dateTime, null, null)
        {
        }

        public TestDateTimeProvider(TimeSpan utcOffset)
            : this(null, utcOffset, null)
        {
        }

        public TestDateTimeProvider(TimeZoneInfo timeZone)
            : this(null, null, timeZone)
        {
        }

        public TestDateTimeProvider(DateTimeOffset dateTime, TimeSpan utcOffset)
            : this(dateTime, utcOffset, null)
        {
        }

        public TestDateTimeProvider(DateTimeOffset dateTime, TimeZoneInfo timeZone)
            : this(dateTime, null, timeZone)
        {
        }

        private TestDateTimeProvider(DateTimeOffset? dateTime, TimeSpan? utcOffset, TimeZoneInfo timeZone)
        {
            _dateTime = dateTime;
            UtcOffset = utcOffset;
            TimeZone = timeZone;
        }

        public DateTimeOffset Now
        {
            get
            {
                var dateTime = _dateTime ?? DateTimeOffset.Now;

                if (UtcOffset != null)
                {
                    dateTime = dateTime.ToOffset(UtcOffset.Value);
                }
                else if (TimeZone != null)
                {
                    dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZone);
                }

                return dateTime;
            }
        }

        public TimeZoneInfo TimeZone { get; set; }

        public DateTimeOffset UtcNow => Now.ToUniversalTime();

        /// <summary>
        /// Overrides <see cref="TimeZone"/> with a set offset from UTC.
        /// </summary>
        public TimeSpan? UtcOffset { get; set; }

        public void AddTime(TimeSpan time)
        {
            SetDateTime(Now.Add(time));
        }

        public void SetDateTime(DateTimeOffset dateTime)
        {
            _dateTime = dateTime;
        }
    }
}

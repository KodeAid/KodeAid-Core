// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Time.Testing;

namespace KodeAid.Testing
{
    /// <summary>
    /// An <see cref="IDateTimeProvider"/> backed by a <see cref="FakeTimeProvider"/>.
    /// The clock is frozen at the date and time it was constructed with,
    /// unless <see cref="FakeTimeProvider.AutoAdvanceAmount"/> is set on <see cref="TimeProvider"/>.
    /// </summary>
    public class TestDateTimeProvider : IDateTimeProvider
    {
        private readonly TimeZoneInfo _defaultTimeZone;
        private TimeZoneInfo? _timeZone;
        private TimeSpan? _utcOffset;

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

        /// <summary>
        /// Wraps an existing <see cref="FakeTimeProvider"/>.
        /// </summary>
        public TestDateTimeProvider(FakeTimeProvider timeProvider)
        {
            ArgCheck.NotNull(nameof(timeProvider), timeProvider);
            TimeProvider = timeProvider;
            _defaultTimeZone = timeProvider.LocalTimeZone;
        }

        private TestDateTimeProvider(DateTimeOffset? dateTime, TimeSpan? utcOffset, TimeZoneInfo? timeZone)
        {
            // A fake time provider must be given a UTC time, otherwise its offset is applied twice.
            TimeProvider = new FakeTimeProvider((dateTime ?? DateTimeOffset.Now).ToUniversalTime());
            _defaultTimeZone = dateTime != null ? CreateFixedTimeZone(dateTime.Value.Offset) : TimeZoneInfo.Local;
            _timeZone = timeZone;
            _utcOffset = utcOffset;
            ApplyTimeZone();
        }

        public DateTimeOffset Now => TimeProvider.GetLocalNow();

        /// <summary>
        /// The underlying fake time provider, which can be passed to a <c>PeriodicTimer</c> or driven directly.
        /// </summary>
        public FakeTimeProvider TimeProvider { get; }

        /// <summary>
        /// The effective time zone, which may be overridden by <see cref="UtcOffset"/>.
        /// </summary>
        [AllowNull]
        public TimeZoneInfo TimeZone
        {
            get => TimeProvider.LocalTimeZone;
            set
            {
                _timeZone = value;
                ApplyTimeZone();
            }
        }

        public DateTimeOffset UtcNow => TimeProvider.GetUtcNow();

        /// <summary>
        /// Overrides <see cref="TimeZone"/> with a set offset from UTC.
        /// </summary>
        public TimeSpan? UtcOffset
        {
            get => _utcOffset;
            set
            {
                _utcOffset = value;
                ApplyTimeZone();
            }
        }

        TimeProvider IDateTimeProvider.TimeProvider => TimeProvider;

        public void AddTime(TimeSpan time)
        {
            if (time < TimeSpan.Zero)
            {
                SetDateTime(UtcNow.Add(time));
            }
            else
            {
                TimeProvider.Advance(time);
            }
        }

        public void SetDateTime(DateTimeOffset dateTime)
        {
            var utcDateTime = dateTime.ToUniversalTime();

            if (utcDateTime < TimeProvider.GetUtcNow())
            {
                // Advance() and SetUtcNow() refuse to move backwards in time, AdjustTime() is how a clock is wound back.
                TimeProvider.AdjustTime(utcDateTime);
            }
            else
            {
                TimeProvider.SetUtcNow(utcDateTime);
            }
        }

        private void ApplyTimeZone()
        {
            TimeProvider.SetLocalTimeZone(
                _utcOffset != null ? CreateFixedTimeZone(_utcOffset.Value) :
                _timeZone ??
                _defaultTimeZone);
        }

        private static TimeZoneInfo CreateFixedTimeZone(TimeSpan utcOffset)
        {
            if (utcOffset == TimeSpan.Zero)
            {
                return TimeZoneInfo.Utc;
            }

            var duration = utcOffset.Duration();
            var id = $"UTC{(utcOffset < TimeSpan.Zero ? "-" : "+")}{duration.Hours:00}:{duration.Minutes:00}";

            return TimeZoneInfo.CreateCustomTimeZone(id, utcOffset, id, id);
        }
    }
}

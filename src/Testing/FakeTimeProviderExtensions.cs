// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using Microsoft.Extensions.Time.Testing;

namespace KodeAid.Testing
{
    /// <summary>
    /// Helpers for driving a <see cref="FakeTimeProvider"/>, which each return the provider so they can be chained,
    /// such as <c>new FakeTimeProvider().SetDateTime(dateTime)</c>.
    /// </summary>
    public static class FakeTimeProviderExtensions
    {
        /// <summary>
        /// Sets the date and time, in either direction.
        /// </summary>
        public static FakeTimeProvider SetDateTime(this FakeTimeProvider timeProvider, DateTimeOffset dateTime)
        {
            ArgCheck.NotNull(nameof(timeProvider), timeProvider);

            // A fake time provider must be given a UTC time, otherwise its offset is applied twice.
            var utcDateTime = dateTime.ToUniversalTime();

            if (utcDateTime < timeProvider.GetUtcNow())
            {
                // Advance() and SetUtcNow() refuse to move backwards in time, AdjustTime() is how a clock is wound back.
                timeProvider.AdjustTime(utcDateTime);
            }
            else
            {
                timeProvider.SetUtcNow(utcDateTime);
            }

            return timeProvider;
        }

        /// <summary>
        /// Moves the clock by the given amount of time, forwards or backwards.
        /// </summary>
        public static FakeTimeProvider AddTime(this FakeTimeProvider timeProvider, TimeSpan time)
        {
            ArgCheck.NotNull(nameof(timeProvider), timeProvider);

            if (time < TimeSpan.Zero)
            {
                return timeProvider.SetDateTime(timeProvider.GetUtcNow().Add(time));
            }

            timeProvider.Advance(time);

            return timeProvider;
        }

        /// <summary>
        /// Sets the local time zone to a fixed offset from UTC.
        /// </summary>
        public static FakeTimeProvider SetUtcOffset(this FakeTimeProvider timeProvider, TimeSpan utcOffset)
        {
            ArgCheck.NotNull(nameof(timeProvider), timeProvider);

            timeProvider.SetLocalTimeZone(CreateFixedTimeZone(utcOffset));

            return timeProvider;
        }

        /// <summary>
        /// Sets the local time zone.
        /// </summary>
        public static FakeTimeProvider SetTimeZone(this FakeTimeProvider timeProvider, TimeZoneInfo timeZone)
        {
            ArgCheck.NotNull(nameof(timeProvider), timeProvider);
            ArgCheck.NotNull(nameof(timeZone), timeZone);

            timeProvider.SetLocalTimeZone(timeZone);

            return timeProvider;
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

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    /// <summary>
    /// An <see cref="IDateTimeProvider"/> backed by a <see cref="TimeProvider"/>,
    /// defaulting to the system clock.
    /// </summary>
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// A shared provider backed by the system clock, mirroring <c>TimeProvider.System</c>.
        /// </summary>
        public static IDateTimeProvider System { get; } = new DateTimeProvider();

        public DateTimeProvider()
            : this(null)
        {
        }

        public DateTimeProvider(TimeProvider? timeProvider)
        {
            TimeProvider = timeProvider ?? TimeProvider.System;
        }

        public DateTimeOffset Now => TimeProvider.GetLocalNow();

        public TimeProvider TimeProvider { get; }

        public TimeZoneInfo TimeZone => TimeProvider.LocalTimeZone;

        public DateTimeOffset UtcNow => TimeProvider.GetUtcNow();
    }
}

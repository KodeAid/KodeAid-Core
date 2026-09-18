// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    /// <summary>
    /// An <see cref="IDateTimeProvider"/> backed by a <see cref="System.TimeProvider"/>,
    /// defaulting to <see cref="System.TimeProvider.System"/>.
    /// </summary>
    public sealed class DefaultDateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// A shared instance backed by <see cref="System.TimeProvider.System"/>.
        /// </summary>
        public static IDateTimeProvider Instance { get; } = new DefaultDateTimeProvider();

        public DefaultDateTimeProvider()
            : this(null)
        {
        }

        public DefaultDateTimeProvider(TimeProvider? timeProvider)
        {
            TimeProvider = timeProvider ?? TimeProvider.System;
        }

        /// <summary>
        /// The underlying time provider.
        /// </summary>
        public TimeProvider TimeProvider { get; }

        public DateTimeOffset Now => TimeProvider.GetLocalNow();

        public TimeZoneInfo TimeZone => TimeProvider.LocalTimeZone;

        public DateTimeOffset UtcNow => TimeProvider.GetUtcNow();
    }
}

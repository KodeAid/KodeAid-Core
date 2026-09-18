// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
        TimeZoneInfo TimeZone { get; }
        DateTimeOffset UtcNow { get; }

        /// <summary>
        /// The clock behind this provider, for creating timers and delays which run on the same clock,
        /// such as <c>new PeriodicTimer(period, dateTimeProvider.TimeProvider)</c>.
        /// </summary>
        TimeProvider TimeProvider { get; }
    }
}

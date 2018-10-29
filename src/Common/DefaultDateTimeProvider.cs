// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    public sealed class DefaultDateTimeProvider : IDateTimeProvider
    {
        public static IDateTimeProvider Instance { get; } = new DefaultDateTimeProvider();

        private DefaultDateTimeProvider() { }

        public DateTimeOffset Now => DateTimeOffset.Now;
        public TimeZoneInfo TimeZone => TimeZoneInfo.Local;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}

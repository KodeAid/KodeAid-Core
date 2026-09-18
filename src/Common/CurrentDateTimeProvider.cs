// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    /// <summary>
    /// An <see cref="IDateTimeProvider"/> which forwards to <see cref="DateTimeProvider.Current"/> on every call,
    /// so that it also picks up providers scoped by <see cref="DateTimeProvider.UseProvider"/>.
    /// This allows injected consumers to follow the ambient provider rather than a provider captured at registration.
    /// </summary>
    public sealed class CurrentDateTimeProvider : IDateTimeProvider
    {
        public static IDateTimeProvider Instance { get; } = new CurrentDateTimeProvider();

        private CurrentDateTimeProvider() { }

        public DateTimeOffset Now => DateTimeProvider.Current.Now;

        public TimeProvider TimeProvider => DateTimeProvider.Current.TimeProvider;

        public TimeZoneInfo TimeZone => DateTimeProvider.Current.TimeZone;

        public DateTimeOffset UtcNow => DateTimeProvider.Current.UtcNow;
    }
}

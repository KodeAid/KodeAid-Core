using System;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace KodeAid
{
    /// <summary>
    /// <see cref="DefaultDateTimeProvider"/> now wraps a <see cref="TimeProvider"/>,
    /// these confirm it still reports the same values as the previous
    /// <see cref="DateTimeOffset.Now"/>/<see cref="DateTimeOffset.UtcNow"/>/<see cref="TimeZoneInfo.Local"/> implementation.
    /// </summary>
    public class DefaultDateTimeProviderTest
    {
        private static readonly TimeSpan _tolerance = TimeSpan.FromSeconds(10);

        [Fact]
        public void NowMatchesSystemLocalTime()
        {
            var expected = DateTimeOffset.Now;

            var actual = DefaultDateTimeProvider.Instance.Now;

            Assert.Equal(expected.Offset, actual.Offset);
            Assert.True((actual - expected).Duration() < _tolerance, $"Expected {actual:o} to be within {_tolerance} of {expected:o}.");
        }

        [Fact]
        public void UtcNowMatchesSystemUtcTime()
        {
            var expected = DateTimeOffset.UtcNow;

            var actual = DefaultDateTimeProvider.Instance.UtcNow;

            Assert.Equal(TimeSpan.Zero, actual.Offset);
            Assert.True((actual - expected).Duration() < _tolerance, $"Expected {actual:o} to be within {_tolerance} of {expected:o}.");
        }

        [Fact]
        public void TimeZoneMatchesSystemLocalTimeZone()
        {
            Assert.Equal(TimeZoneInfo.Local, DefaultDateTimeProvider.Instance.TimeZone);
        }

        [Fact]
        public void InstanceIsASingleton()
        {
            // the ambient provider is covered by DateTimeProviderTest, which owns the process-wide state
            Assert.Same(DefaultDateTimeProvider.Instance, DefaultDateTimeProvider.Instance);
        }

        [Fact]
        public void InstanceIsBackedBySystemTimeProvider()
        {
            var provider = Assert.IsType<DefaultDateTimeProvider>(DefaultDateTimeProvider.Instance);

            Assert.Same(TimeProvider.System, provider.TimeProvider);
        }

        [Fact]
        public void ParameterlessConstructorUsesSystemTimeProvider()
        {
            var provider = new DefaultDateTimeProvider();

            Assert.Same(TimeProvider.System, provider.TimeProvider);
        }

        [Fact]
        public void NullTimeProviderFallsBackToSystemTimeProvider()
        {
            var provider = new DefaultDateTimeProvider(null);

            Assert.Same(TimeProvider.System, provider.TimeProvider);
        }

        [Fact]
        public void SuppliedTimeProviderDrivesTheClock()
        {
            var timeProvider = new FakeTimeProvider(new DateTimeOffset(2019, 7, 4, 16, 30, 0, TimeSpan.Zero));
            timeProvider.SetLocalTimeZone(TimeZoneInfo.CreateCustomTimeZone("UTC-06:00", TimeSpan.FromHours(-6), "UTC-06:00", "UTC-06:00"));

            var provider = new DefaultDateTimeProvider(timeProvider);

            Assert.Same(timeProvider, provider.TimeProvider);
            Assert.Equal(new DateTimeOffset(2019, 7, 4, 16, 30, 0, TimeSpan.Zero), provider.UtcNow);
            Assert.Equal(new DateTimeOffset(2019, 7, 4, 10, 30, 0, TimeSpan.FromHours(-6)), provider.Now);
            Assert.Equal(timeProvider.LocalTimeZone, provider.TimeZone);
        }
    }
}

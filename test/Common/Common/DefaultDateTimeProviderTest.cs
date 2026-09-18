using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace KodeAid
{
    /// <summary>
    /// <see cref="DefaultDateTimeProvider"/> is now a <see cref="TimeProvider"/>,
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
            // The ambient provider is covered by DateTimeProviderTest, which owns the process-wide state.
            Assert.Same(DefaultDateTimeProvider.Instance, DefaultDateTimeProvider.Instance);
        }

        [Fact]
        public void ExposesTheSystemTimeProvider()
        {
            Assert.Same(TimeProvider.System, DefaultDateTimeProvider.Instance.TimeProvider);
            Assert.Same(TimeProvider.System, new DefaultDateTimeProvider().TimeProvider);
            Assert.Same(TimeProvider.System, new DefaultDateTimeProvider(null).TimeProvider);
        }

        [Fact]
        public void ParameterlessConstructorUsesTheSystemClock()
        {
            var provider = new DefaultDateTimeProvider();

            AssertWithinTolerance(DateTimeOffset.UtcNow, provider.UtcNow);
            Assert.Equal(TimeZoneInfo.Local, provider.TimeZone);
        }

        [Fact]
        public void NullTimeProviderFallsBackToTheSystemClock()
        {
            var provider = new DefaultDateTimeProvider(null);

            AssertWithinTolerance(DateTimeOffset.UtcNow, provider.UtcNow);
            Assert.Equal(TimeZoneInfo.Local, provider.TimeZone);
        }

        [Fact]
        public void SuppliedTimeProviderDrivesTheClock()
        {
            var timeProvider = new FakeTimeProvider(new DateTimeOffset(2019, 7, 4, 16, 30, 0, TimeSpan.Zero));
            timeProvider.SetLocalTimeZone(TimeZoneInfo.CreateCustomTimeZone("UTC-06:00", TimeSpan.FromHours(-6), "UTC-06:00", "UTC-06:00"));

            var provider = new DefaultDateTimeProvider(timeProvider);

            Assert.Equal(new DateTimeOffset(2019, 7, 4, 16, 30, 0, TimeSpan.Zero), provider.UtcNow);
            Assert.Equal(new DateTimeOffset(2019, 7, 4, 10, 30, 0, TimeSpan.FromHours(-6)), provider.Now);
            Assert.Equal(TimeSpan.FromHours(-6), provider.Now.Offset);
            Assert.Equal(timeProvider.LocalTimeZone, provider.TimeZone);
            Assert.Same(timeProvider, provider.TimeProvider);
        }

        [Fact]
        public async Task SuppliedTimeProviderAlsoDrivesTimers()
        {
            var timeProvider = new FakeTimeProvider(new DateTimeOffset(2019, 7, 4, 16, 30, 0, TimeSpan.Zero));

            var provider = new DefaultDateTimeProvider(timeProvider);

            // The timer must run on the supplied clock, not the real one.
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30), provider.TimeProvider);

            var tick = timer.WaitForNextTickAsync();

            Assert.False(tick.IsCompleted);

            timeProvider.Advance(TimeSpan.FromSeconds(30));

            Assert.True(await tick);
        }

        private static void AssertWithinTolerance(DateTimeOffset expected, DateTimeOffset actual)
        {
            Assert.True((actual - expected).Duration() < _tolerance, $"Expected {actual:o} to be within {_tolerance} of {expected:o}.");
        }
    }
}

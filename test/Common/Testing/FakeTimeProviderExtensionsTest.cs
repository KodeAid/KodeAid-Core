using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace KodeAid.Testing
{
    /// <summary>
    /// These cover the two things which are easy to get wrong when driving a <see cref="FakeTimeProvider"/>:
    /// it must be given a UTC time, and it refuses to move backwards through <c>SetUtcNow</c> or <c>Advance</c>.
    /// </summary>
    public class FakeTimeProviderExtensionsTest
    {
        private static readonly DateTimeOffset _dateTime = new DateTimeOffset(2019, 7, 4, 10, 30, 0, TimeSpan.FromHours(-6));

        [Fact]
        public void SetDateTimeNormalizesToUniversalTime()
        {
            // Passing an offset date and time to the constructor would apply the offset twice.
            var timeProvider = new FakeTimeProvider().SetDateTime(_dateTime);

            Assert.Equal(_dateTime, timeProvider.GetUtcNow());
            Assert.Equal(TimeSpan.Zero, timeProvider.GetUtcNow().Offset);
            Assert.Equal(_dateTime, new DateTimeProvider(timeProvider).UtcNow);
        }

        [Fact]
        public void SetDateTimeMovesTheClockForward()
        {
            var timeProvider = new FakeTimeProvider().SetDateTime(_dateTime);

            timeProvider.SetDateTime(_dateTime.AddHours(3));

            Assert.Equal(_dateTime.AddHours(3), timeProvider.GetUtcNow());
        }

        [Fact]
        public void SetDateTimeMovesTheClockBackward()
        {
            var timeProvider = new FakeTimeProvider().SetDateTime(_dateTime);

            timeProvider.SetDateTime(_dateTime.AddDays(-30));

            Assert.Equal(_dateTime.AddDays(-30), timeProvider.GetUtcNow());
        }

        [Fact]
        public void AddTimeAdvancesTheClock()
        {
            var timeProvider = new FakeTimeProvider().SetDateTime(_dateTime);

            timeProvider.AddTime(TimeSpan.FromMinutes(90));

            Assert.Equal(_dateTime.AddMinutes(90), timeProvider.GetUtcNow());
        }

        [Fact]
        public void AddTimeWithANegativeValueRewindsTheClock()
        {
            var timeProvider = new FakeTimeProvider().SetDateTime(_dateTime);

            timeProvider.AddTime(TimeSpan.FromMinutes(-90));

            Assert.Equal(_dateTime.AddMinutes(-90), timeProvider.GetUtcNow());
        }

        [Fact]
        public void AddTimeIsCumulative()
        {
            var timeProvider = new FakeTimeProvider().SetDateTime(_dateTime);

            timeProvider
                .AddTime(TimeSpan.FromHours(2))
                .AddTime(TimeSpan.FromHours(-5))
                .AddTime(TimeSpan.FromHours(1));

            Assert.Equal(_dateTime.AddHours(-2), timeProvider.GetUtcNow());
        }

        [Fact]
        public void SetUtcOffsetReportsTheLocalTimeAtThatOffset()
        {
            var timeProvider = new FakeTimeProvider()
                .SetDateTime(_dateTime)
                .SetUtcOffset(TimeSpan.FromHours(5.5));

            var provider = new DateTimeProvider(timeProvider);

            Assert.Equal(TimeSpan.FromHours(5.5), provider.Now.Offset);
            Assert.Equal(_dateTime.ToOffset(TimeSpan.FromHours(5.5)), provider.Now);
            Assert.Equal(_dateTime, provider.UtcNow);
        }

        [Fact]
        public void SetUtcOffsetOfZeroIsUtc()
        {
            var timeProvider = new FakeTimeProvider().SetUtcOffset(TimeSpan.Zero);

            Assert.Equal(TimeZoneInfo.Utc, timeProvider.LocalTimeZone);
        }

        [Fact]
        public void SetTimeZoneReportsTheLocalTimeInThatZone()
        {
            var timeZone = TimeZoneInfo.CreateCustomTimeZone("UTC+10:00", TimeSpan.FromHours(10), "UTC+10:00", "UTC+10:00");

            var timeProvider = new FakeTimeProvider()
                .SetDateTime(_dateTime)
                .SetTimeZone(timeZone);

            var provider = new DateTimeProvider(timeProvider);

            Assert.Equal(TimeZoneInfo.ConvertTime(_dateTime, timeZone), provider.Now);
            Assert.Equal(timeZone, provider.TimeZone);
        }

        [Fact]
        public async Task DrivesAPeriodicTimer()
        {
            var timeProvider = new FakeTimeProvider().SetDateTime(_dateTime);

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30), timeProvider);

            var tick = timer.WaitForNextTickAsync();

            timeProvider.AddTime(TimeSpan.FromSeconds(29));

            Assert.False(tick.IsCompleted);

            timeProvider.AddTime(TimeSpan.FromSeconds(1));

            Assert.True(await tick);
        }

        [Fact]
        public async Task RewindingTheClockKeepsExistingTimersRunning()
        {
            var timeProvider = new FakeTimeProvider().SetDateTime(_dateTime);

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30), timeProvider);

            var tick = timer.WaitForNextTickAsync();

            timeProvider.SetDateTime(_dateTime.AddDays(-30));

            Assert.False(tick.IsCompleted);

            timeProvider.AddTime(TimeSpan.FromSeconds(30));

            Assert.True(await tick);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace KodeAid.Testing
{
    /// <summary>
    /// <see cref="TestDateTimeProvider"/> is now a <see cref="FakeTimeProvider"/>,
    /// these confirm the values it reports are the same as the previous hand-rolled implementation, which was:
    /// <code>
    /// Now     => (_dateTime ?? DateTimeOffset.Now) converted by UtcOffset, else by TimeZone, else left as-is
    /// UtcNow  => Now.ToUniversalTime()
    /// AddTime => SetDateTime(Now.Add(time))
    /// </code>
    /// </summary>
    public class TestDateTimeProviderTest
    {
        private static readonly DateTimeOffset _dateTime = new DateTimeOffset(2019, 7, 4, 10, 30, 0, TimeSpan.FromHours(-6));
        private static readonly TimeSpan _tolerance = TimeSpan.FromSeconds(10);

        #region behavior carried over from the previous implementation

        [Fact]
        public void DateTimeIsReportedWithItsOriginalOffset()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            AssertEqual(_dateTime, provider.Now);
            AssertEqual(_dateTime.ToUniversalTime(), provider.UtcNow);
        }

        [Fact]
        public void UtcOffsetAloneIsAppliedToTheCurrentTime()
        {
            var utcOffset = TimeSpan.FromHours(5.5);
            var expected = DateTimeOffset.Now.ToOffset(utcOffset);

            var provider = new TestDateTimeProvider(utcOffset);

            Assert.Equal(utcOffset, provider.Now.Offset);
            AssertWithinTolerance(expected, provider.Now);
        }

        [Fact]
        public void TimeZoneAloneIsAppliedToTheCurrentTime()
        {
            var timeZone = CreateFixedTimeZone(TimeSpan.FromHours(-6));
            var expected = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, timeZone);

            var provider = new TestDateTimeProvider(timeZone);

            Assert.Equal(expected.Offset, provider.Now.Offset);
            AssertWithinTolerance(expected, provider.Now);
        }

        [Fact]
        public void DateTimeIsConvertedToTheUtcOffset()
        {
            var utcOffset = TimeSpan.FromHours(5.5);

            var provider = new TestDateTimeProvider(_dateTime, utcOffset);

            AssertEqual(_dateTime.ToOffset(utcOffset), provider.Now);
            AssertEqual(_dateTime.ToUniversalTime(), provider.UtcNow);
        }

        [Fact]
        public void DateTimeIsConvertedToTheTimeZone()
        {
            var timeZone = CreateFixedTimeZone(TimeSpan.FromHours(10));

            var provider = new TestDateTimeProvider(_dateTime, timeZone);

            AssertEqual(TimeZoneInfo.ConvertTime(_dateTime, timeZone), provider.Now);
            AssertEqual(_dateTime.ToUniversalTime(), provider.UtcNow);
        }

        [Theory]
        [InlineData(7)]  // Daylight saving time.
        [InlineData(1)]  // Standard time.
        public void DateTimeIsConvertedToTheTimeZoneHonoringDaylightSavingTime(int month)
        {
            var dateTime = new DateTimeOffset(2019, month, 4, 16, 30, 0, TimeSpan.Zero);
            var timeZone = CreateDaylightSavingTimeZone();

            var provider = new TestDateTimeProvider(dateTime, timeZone);

            AssertEqual(TimeZoneInfo.ConvertTime(dateTime, timeZone), provider.Now);
        }

        [Fact]
        public void UtcOffsetOverridesTimeZone()
        {
            var timeZone = CreateFixedTimeZone(TimeSpan.FromHours(-6));
            var utcOffset = TimeSpan.FromHours(3);

            var provider = new TestDateTimeProvider(_dateTime, timeZone)
            {
                UtcOffset = utcOffset,
            };

            AssertEqual(_dateTime.ToOffset(utcOffset), provider.Now);

            // Clearing the offset falls back to the time zone.
            provider.UtcOffset = null;

            AssertEqual(TimeZoneInfo.ConvertTime(_dateTime, timeZone), provider.Now);
        }

        [Fact]
        public void TimeZoneCanBeChangedAfterConstruction()
        {
            var timeZone = CreateFixedTimeZone(TimeSpan.FromHours(10));

            var provider = new TestDateTimeProvider(_dateTime)
            {
                TimeZone = timeZone,
            };

            AssertEqual(TimeZoneInfo.ConvertTime(_dateTime, timeZone), provider.Now);

            // Clearing the time zone falls back to the offset the provider was constructed with.
            provider.TimeZone = null;

            AssertEqual(_dateTime, provider.Now);
        }

        [Fact]
        public void UtcNowIsAlwaysNowInUniversalTime()
        {
            var provider = new TestDateTimeProvider(_dateTime, CreateFixedTimeZone(TimeSpan.FromHours(10)));

            AssertEqual(provider.Now.ToUniversalTime(), provider.UtcNow);
            Assert.Equal(TimeSpan.Zero, provider.UtcNow.Offset);
        }

        [Fact]
        public void SetDateTimeMovesTheClockForward()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            provider.SetDateTime(_dateTime.AddHours(3));

            AssertEqual(_dateTime.AddHours(3), provider.Now);
        }

        [Fact]
        public void SetDateTimeMovesTheClockBackward()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            provider.SetDateTime(_dateTime.AddDays(-30));

            AssertEqual(_dateTime.AddDays(-30), provider.Now);
        }

        [Fact]
        public void SetDateTimeKeepsTheConfiguredTimeZone()
        {
            var utcOffset = TimeSpan.FromHours(3);
            var provider = new TestDateTimeProvider(_dateTime, utcOffset);

            provider.SetDateTime(_dateTime.AddDays(-30));

            AssertEqual(_dateTime.AddDays(-30).ToOffset(utcOffset), provider.Now);
        }

        [Fact]
        public void AddTimeAdvancesTheClock()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            provider.AddTime(TimeSpan.FromMinutes(90));

            AssertEqual(_dateTime.AddMinutes(90), provider.Now);
        }

        [Fact]
        public void AddTimeWithANegativeValueRewindsTheClock()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            provider.AddTime(TimeSpan.FromMinutes(-90));

            AssertEqual(_dateTime.AddMinutes(-90), provider.Now);
        }

        [Fact]
        public void AddTimeIsCumulative()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            provider.AddTime(TimeSpan.FromHours(2));
            provider.AddTime(TimeSpan.FromHours(-5));
            provider.AddTime(TimeSpan.FromHours(1));

            AssertEqual(_dateTime.AddHours(-2), provider.Now);
        }

        [Fact]
        public void CanBeUsedAsTheCurrentDateTimeProvider()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            // Scoped rather than set process-wide, so this cannot disturb tests running in parallel.
            using (DateTimeProvider.UseProvider(provider))
            {
                AssertEqual(_dateTime, DateTimeProvider.Current.Now);
                AssertEqual(_dateTime.ToUniversalTime(), DateTimeProvider.Current.UtcNow);
            }
        }

        #endregion

        #region intentional changes

        /// <summary>
        /// Previously the clock kept ticking along with the real clock until a date and time was set,
        /// it is now frozen at the moment of construction.
        /// </summary>
        [Fact]
        public void ClockIsFrozenAtConstruction()
        {
            var provider = new TestDateTimeProvider();

            AssertWithinTolerance(DateTimeOffset.Now, provider.Now);
            Assert.Equal(TimeZoneInfo.Local.GetUtcOffset(provider.UtcNow), provider.Now.Offset);

            var first = provider.Now;
            var second = provider.Now;

            AssertEqual(first, second);
        }

        /// <summary>
        /// Previously this returned null until a time zone was explicitly set,
        /// it now reports the effective time zone so it satisfies <see cref="IDateTimeProvider.TimeZone"/>.
        /// </summary>
        [Fact]
        public void TimeZoneReportsTheEffectiveTimeZone()
        {
            Assert.Equal(TimeZoneInfo.Local, new TestDateTimeProvider().TimeZone);
            Assert.Equal(_dateTime.Offset, new TestDateTimeProvider(_dateTime).TimeZone.BaseUtcOffset);
            Assert.Equal(TimeSpan.FromHours(3), new TestDateTimeProvider(_dateTime, TimeSpan.FromHours(3)).TimeZone.BaseUtcOffset);
            Assert.Equal(TimeZoneInfo.Utc, new TestDateTimeProvider(_dateTime, TimeSpan.Zero).TimeZone);
        }

        #endregion

        #region fake time provider

        [Fact]
        public void ExposesItsFakeTimeProviderThroughTheInterface()
        {
            IDateTimeProvider provider = new TestDateTimeProvider(_dateTime);

            Assert.IsType<FakeTimeProvider>(provider.TimeProvider);
            Assert.Same(((TestDateTimeProvider)provider).TimeProvider, provider.TimeProvider);
        }

        [Fact]
        public void TheWrappedClockAgreesWithTheProvider()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            AssertEqual(provider.TimeProvider.GetUtcNow(), provider.UtcNow);
            AssertEqual(provider.TimeProvider.GetLocalNow(), provider.Now);
            Assert.Equal(provider.TimeProvider.LocalTimeZone, provider.TimeZone);

            provider.TimeProvider.Advance(TimeSpan.FromHours(4));

            AssertEqual(_dateTime.AddHours(4), provider.Now);
        }

        [Fact]
        public void AddTimeAndSetDateTimeMoveTheWrappedClock()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            provider.AddTime(TimeSpan.FromHours(4));

            Assert.Equal(_dateTime.AddHours(4), provider.TimeProvider.GetUtcNow());

            provider.SetDateTime(_dateTime.AddHours(6));

            Assert.Equal(_dateTime.AddHours(6), provider.TimeProvider.GetUtcNow());
        }

        [Fact]
        public void AutoAdvanceMakesTheClockTick()
        {
            var provider = new TestDateTimeProvider(_dateTime);
            provider.TimeProvider.AutoAdvanceAmount = TimeSpan.FromSeconds(1);

            AssertEqual(_dateTime, provider.Now);
            AssertEqual(_dateTime.AddSeconds(1), provider.Now);
            AssertEqual(_dateTime.AddSeconds(2), provider.Now);
        }

        [Fact]
        public async Task DrivesAPeriodicTimer()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30), provider.TimeProvider);

            var tick = timer.WaitForNextTickAsync();

            provider.AddTime(TimeSpan.FromSeconds(29));

            Assert.False(tick.IsCompleted);

            provider.AddTime(TimeSpan.FromSeconds(1));

            Assert.True(await tick);
            AssertEqual(_dateTime.AddSeconds(30), provider.Now);
        }

        [Fact]
        public async Task DrivesDelaysAndTimeouts()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            var delay = Task.Delay(TimeSpan.FromMinutes(5), provider.TimeProvider);
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10), provider.TimeProvider);

            provider.AddTime(TimeSpan.FromMinutes(5));

            await delay;

            Assert.False(cts.IsCancellationRequested);

            provider.AddTime(TimeSpan.FromMinutes(5));

            Assert.True(cts.IsCancellationRequested);
        }

        /// <summary>
        /// Rewinding used to replace the wrapped fake, which orphaned any timer already created from it.
        /// </summary>
        [Fact]
        public async Task RewindingTheClockKeepsExistingTimersRunning()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30), provider.TimeProvider);

            var tick = timer.WaitForNextTickAsync();

            provider.SetDateTime(_dateTime.AddDays(-30));

            AssertEqual(_dateTime.AddDays(-30), provider.Now);
            Assert.False(tick.IsCompleted);

            provider.AddTime(TimeSpan.FromSeconds(30));

            Assert.True(await tick);
        }

        #endregion

        private static void AssertEqual(DateTimeOffset expected, DateTimeOffset actual)
        {
            // Equality on DateTimeOffset only compares the instant in time, so the offset is checked separately.
            Assert.Equal(expected, actual);
            Assert.Equal(expected.Offset, actual.Offset);
        }

        private static void AssertWithinTolerance(DateTimeOffset expected, DateTimeOffset actual)
        {
            Assert.True((actual - expected).Duration() < _tolerance, $"Expected {actual:o} to be within {_tolerance} of {expected:o}.");
        }

        private static TimeZoneInfo CreateFixedTimeZone(TimeSpan utcOffset)
        {
            var id = $"Test {utcOffset}";

            return TimeZoneInfo.CreateCustomTimeZone(id, utcOffset, id, id);
        }

        private static TimeZoneInfo CreateDaylightSavingTimeZone()
        {
            var start = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 15);
            var end = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1);
            var rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(DateTime.MinValue.Date, DateTime.MaxValue.Date, TimeSpan.FromHours(1), start, end);

            return TimeZoneInfo.CreateCustomTimeZone("Test Daylight Time", TimeSpan.FromHours(-6), "Test Daylight Time", "Test Standard Time", "Test Summer Time", new[] { rule });
        }
    }
}

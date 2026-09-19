using System;
using System.Linq;
using KodeAid;
using KodeAid.Testing;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace KodeAid
{
    public class ServiceCollectionExtensionsTest
    {
        private static readonly DateTimeOffset _dateTime = new DateTimeOffset(2019, 7, 4, 10, 30, 0, TimeSpan.FromHours(-6));

        [Fact]
        public void AddDateTimeProviderFollowsTheRegisteredTimeProvider()
        {
            var timeProvider = new FakeTimeProvider(_dateTime.ToUniversalTime());

            using var serviceProvider = new ServiceCollection()
                .AddSingleton<TimeProvider>(timeProvider)
                .AddDateTimeProvider()
                .BuildServiceProvider();

            var dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

            Assert.Same(timeProvider, dateTimeProvider.TimeProvider);
            Assert.Equal(_dateTime, dateTimeProvider.UtcNow);

            timeProvider.Advance(TimeSpan.FromHours(3));

            Assert.Equal(_dateTime.AddHours(3), dateTimeProvider.UtcNow);
        }

        [Fact]
        public void AddDateTimeProviderPicksUpATimeProviderRegisteredAfterIt()
        {
            var timeProvider = new FakeTimeProvider(_dateTime.ToUniversalTime());

            using var serviceProvider = new ServiceCollection()
                .AddDateTimeProvider()
                .AddSingleton<TimeProvider>(timeProvider)
                .BuildServiceProvider();

            // Both are resolved lazily, so the order they were registered in does not matter.
            Assert.Same(timeProvider, serviceProvider.GetRequiredService<IDateTimeProvider>().TimeProvider);
        }

        [Fact]
        public void AddDateTimeProviderFallsBackToTheSystemClock()
        {
            using var serviceProvider = new ServiceCollection().AddDateTimeProvider().BuildServiceProvider();

            var dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

            Assert.Same(TimeProvider.System, dateTimeProvider.TimeProvider);
            Assert.Equal(TimeZoneInfo.Local, dateTimeProvider.TimeZone);
        }

        [Fact]
        public void AddDateTimeProviderDoesNotReplaceAnExistingRegistration()
        {
            var provider = new DateTimeProvider(new FakeTimeProvider().SetDateTime(_dateTime));

            using var serviceProvider = new ServiceCollection()
                .AddSingleton<IDateTimeProvider>(provider)
                .AddDateTimeProvider()
                .BuildServiceProvider();

            Assert.Same(provider, serviceProvider.GetRequiredService<IDateTimeProvider>());
            Assert.Single(serviceProvider.GetServices<IDateTimeProvider>());
        }
    }
}

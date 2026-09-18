using System;
using System.Linq;
using KodeAid;
using KodeAid.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace KodeAid
{
    public class ServiceCollectionExtensionsTest
    {
        private static readonly DateTimeOffset _dateTime = new DateTimeOffset(2019, 7, 4, 10, 30, 0, TimeSpan.FromHours(-6));

        [Fact]
        public void AddCurrentDateTimeProviderRegistersTheInterface()
        {
            var services = new ServiceCollection().AddCurrentDateTimeProvider();

            var descriptor = Assert.Single(services);

            Assert.Equal(typeof(IDateTimeProvider), descriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        [Fact]
        public void AddCurrentDateTimeProviderResolvesAProviderWhichFollowsTheAmbientProvider()
        {
            using var serviceProvider = new ServiceCollection().AddCurrentDateTimeProvider().BuildServiceProvider();

            // Resolved before the scope is opened, as a singleton consumer would be.
            var dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

            var provider = new TestDateTimeProvider(_dateTime);

            using (DateTimeProvider.UseProvider(provider))
            {
                Assert.Equal(_dateTime, dateTimeProvider.Now);
                Assert.Equal(_dateTime.Offset, dateTimeProvider.Now.Offset);

                provider.AddTime(TimeSpan.FromHours(3));

                Assert.Equal(_dateTime.AddHours(3), dateTimeProvider.Now);
            }

            Assert.Equal(TimeZoneInfo.Local, dateTimeProvider.TimeZone);
        }

        [Fact]
        public void AddDefaultDateTimeProviderResolvesTheSystemClock()
        {
            using var serviceProvider = new ServiceCollection().AddDefaultDateTimeProvider().BuildServiceProvider();

            var dateTimeProvider = serviceProvider.GetRequiredService<IDateTimeProvider>();

            Assert.Same(DefaultDateTimeProvider.Instance, dateTimeProvider);

            // Unlike the current provider, this one ignores the ambient provider.
            using (DateTimeProvider.UseProvider(new TestDateTimeProvider(_dateTime)))
            {
                Assert.Equal(TimeZoneInfo.Local, dateTimeProvider.TimeZone);
            }
        }

        [Fact]
        public void TheDateTimeProviderRegistrationsDoNotReplaceAnExistingOne()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            using var serviceProvider = new ServiceCollection()
                .AddSingleton<IDateTimeProvider>(provider)
                .AddCurrentDateTimeProvider()
                .AddDefaultDateTimeProvider()
                .BuildServiceProvider();

            Assert.Same(provider, serviceProvider.GetRequiredService<IDateTimeProvider>());
            Assert.Single(serviceProvider.GetServices<IDateTimeProvider>());
        }
    }
}

using System;
using KodeAid.Testing;
using Xunit;

namespace KodeAid
{
    public class CurrentDateTimeProviderTest
    {
        private static readonly DateTimeOffset _dateTime = new DateTimeOffset(2019, 7, 4, 10, 30, 0, TimeSpan.FromHours(-6));

        [Fact]
        public void InstanceIsASingleton()
        {
            Assert.Same(CurrentDateTimeProvider.Instance, CurrentDateTimeProvider.Instance);
        }

        [Fact]
        public void ForwardsToTheAmbientProvider()
        {
            var provider = new TestDateTimeProvider(_dateTime);

            using (DateTimeProvider.UseProvider(provider))
            {
                Assert.Equal(provider.Now, CurrentDateTimeProvider.Instance.Now);
                Assert.Equal(provider.Now.Offset, CurrentDateTimeProvider.Instance.Now.Offset);
                Assert.Equal(provider.UtcNow, CurrentDateTimeProvider.Instance.UtcNow);
                Assert.Equal(provider.TimeZone, CurrentDateTimeProvider.Instance.TimeZone);
            }
        }

        [Fact]
        public void FollowsTheAmbientProviderOnEveryCallRatherThanCapturingIt()
        {
            // Held before any scope is opened, exactly as an injected consumer would hold it.
            var injected = CurrentDateTimeProvider.Instance;

            Assert.Equal(TimeZoneInfo.Local, injected.TimeZone);

            var provider = new TestDateTimeProvider(_dateTime);

            using (DateTimeProvider.UseProvider(provider))
            {
                Assert.Equal(_dateTime, injected.Now);

                // And it keeps following that provider as its clock moves.
                provider.AddTime(TimeSpan.FromHours(3));

                Assert.Equal(_dateTime.AddHours(3), injected.Now);
            }

            // Back to the real clock once the scope is gone.
            Assert.Equal(TimeZoneInfo.Local, injected.TimeZone);
        }
    }
}

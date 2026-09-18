using System;
using System.Linq;
using System.Threading.Tasks;
using KodeAid.Testing;
using Xunit;

namespace KodeAid
{
    /// <summary>
    /// This is the only test class which touches the process-wide provider, tests within a class are
    /// never run in parallel with each other, so keeping it here is what stops it racing other tests.
    /// Everything else overrides the provider with <see cref="DateTimeProvider.UseProvider"/>,
    /// which is scoped to the running test.
    /// </summary>
    public class DateTimeProviderTest
    {
        [Fact]
        public void CurrentDefaultsToTheDefaultProvider()
        {
            Assert.Same(DefaultDateTimeProvider.Instance, DateTimeProvider.Current);
        }

        [Fact]
        public void SetCurrentProviderReplacesTheProcessWideProvider()
        {
            var provider = new TestDateTimeProvider();

            try
            {
                DateTimeProvider.SetCurrentProvider(provider);

                Assert.Same(provider, DateTimeProvider.Current);
            }
            finally
            {
                DateTimeProvider.ResetCurrentProviderToDefault();
            }

            Assert.Same(DefaultDateTimeProvider.Instance, DateTimeProvider.Current);
        }

        [Fact]
        public void SetCurrentProviderThrowsWhenTheProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => DateTimeProvider.SetCurrentProvider(null!));
        }

        [Fact]
        public void UseProviderThrowsWhenTheProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => DateTimeProvider.UseProvider(null!));
        }

        [Fact]
        public void UseProviderOverridesCurrentUntilTheScopeIsDisposed()
        {
            var provider = new TestDateTimeProvider();

            using (DateTimeProvider.UseProvider(provider))
            {
                Assert.Same(provider, DateTimeProvider.Current);
            }

            Assert.Same(DefaultDateTimeProvider.Instance, DateTimeProvider.Current);
        }

        [Fact]
        public void UseProviderTakesPrecedenceOverTheProcessWideProvider()
        {
            var processWide = new TestDateTimeProvider();
            var scoped = new TestDateTimeProvider();

            try
            {
                DateTimeProvider.SetCurrentProvider(processWide);

                using (DateTimeProvider.UseProvider(scoped))
                {
                    Assert.Same(scoped, DateTimeProvider.Current);
                }

                Assert.Same(processWide, DateTimeProvider.Current);
            }
            finally
            {
                DateTimeProvider.ResetCurrentProviderToDefault();
            }
        }

        [Fact]
        public void ResetCurrentProviderToDefaultLeavesAScopedProviderInEffect()
        {
            var scoped = new TestDateTimeProvider();

            try
            {
                DateTimeProvider.SetCurrentProvider(new TestDateTimeProvider());

                using (DateTimeProvider.UseProvider(scoped))
                {
                    DateTimeProvider.ResetCurrentProviderToDefault();

                    Assert.Same(scoped, DateTimeProvider.Current);
                }

                Assert.Same(DefaultDateTimeProvider.Instance, DateTimeProvider.Current);
            }
            finally
            {
                DateTimeProvider.ResetCurrentProviderToDefault();
            }
        }

        [Fact]
        public void ScopesNest()
        {
            var outer = new TestDateTimeProvider();
            var inner = new TestDateTimeProvider();

            using (DateTimeProvider.UseProvider(outer))
            {
                Assert.Same(outer, DateTimeProvider.Current);

                using (DateTimeProvider.UseProvider(inner))
                {
                    Assert.Same(inner, DateTimeProvider.Current);
                }

                Assert.Same(outer, DateTimeProvider.Current);
            }

            Assert.Same(DefaultDateTimeProvider.Instance, DateTimeProvider.Current);
        }

        [Fact]
        public void DisposingAScopeMoreThanOnceDoesNothing()
        {
            var outer = new TestDateTimeProvider();
            var inner = new TestDateTimeProvider();

            using (DateTimeProvider.UseProvider(outer))
            {
                var scope = DateTimeProvider.UseProvider(inner);

                scope.Dispose();
                scope.Dispose();

                Assert.Same(outer, DateTimeProvider.Current);
            }
        }

        [Fact]
        public async Task ScopeFlowsAcrossAwaits()
        {
            var provider = new TestDateTimeProvider();

            using (DateTimeProvider.UseProvider(provider))
            {
                await Task.Delay(10);

                Assert.Same(provider, DateTimeProvider.Current);

                await Task.Run(() => Assert.Same(provider, DateTimeProvider.Current));

                Assert.Same(provider, DateTimeProvider.Current);
            }
        }

        [Fact]
        public async Task ScopeDoesNotLeakOutOfTheFlowThatOpenedIt()
        {
            var provider = new TestDateTimeProvider();

            await Task.Run(() =>
            {
                using (DateTimeProvider.UseProvider(provider))
                {
                    Assert.Same(provider, DateTimeProvider.Current);
                }
            });

            Assert.Same(DefaultDateTimeProvider.Instance, DateTimeProvider.Current);
        }

        [Fact]
        public async Task ConcurrentFlowsEachSeeTheirOwnProvider()
        {
            var tasks = Enumerable.Range(0, 16).Select(i => Task.Run(async () =>
            {
                var provider = new TestDateTimeProvider(new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).AddDays(i));

                using (DateTimeProvider.UseProvider(provider))
                {
                    for (var j = 0; j < 10; j++)
                    {
                        Assert.Same(provider, DateTimeProvider.Current);

                        // moving one flow's clock must not be visible to any other flow
                        provider.AddTime(TimeSpan.FromHours(1));

                        Assert.Equal(new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).AddDays(i).AddHours(j + 1), DateTimeProvider.Current.UtcNow);

                        await Task.Yield();
                    }
                }
            }));

            await Task.WhenAll(tasks);

            Assert.Same(DefaultDateTimeProvider.Instance, DateTimeProvider.Current);
        }

        [Fact]
        public void IsAStaticClass()
        {
            var type = typeof(DateTimeProvider);

            Assert.True(type.IsAbstract && type.IsSealed, "DateTimeProvider should be a static class.");
        }
    }
}

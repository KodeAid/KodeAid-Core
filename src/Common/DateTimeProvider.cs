// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Threading;

namespace KodeAid
{
    /// <summary>
    /// The ambient <see cref="IDateTimeProvider"/>, for code which cannot take one as a dependency.
    /// Prefer injecting an <see cref="IDateTimeProvider"/> wherever that is possible.
    /// </summary>
    public static class DateTimeProvider
    {
        private static readonly AsyncLocal<IDateTimeProvider?> _scopedProvider = new AsyncLocal<IDateTimeProvider?>();
        private static volatile IDateTimeProvider _defaultProvider = DefaultDateTimeProvider.Instance;

        /// <summary>
        /// The provider scoped to the current asynchronous flow if there is one,
        /// otherwise the process-wide provider.
        /// </summary>
        public static IDateTimeProvider Current => _scopedProvider.Value ?? _defaultProvider;

        /// <summary>
        /// Sets the process-wide provider, intended to be called once during startup.
        /// Every thread sees it, and it stays in effect until it is set or reset again,
        /// use <see cref="UseProvider"/> to override the provider for a single test or operation instead.
        /// </summary>
        public static void SetCurrentProvider(IDateTimeProvider provider)
        {
            CheckProvider(nameof(provider), provider);
            _defaultProvider = provider;
        }

        /// <summary>
        /// Resets the process-wide provider back to <see cref="DefaultDateTimeProvider.Instance"/>.
        /// Providers scoped by <see cref="UseProvider"/> are left in effect.
        /// </summary>
        public static void ResetCurrentProviderToDefault()
        {
            _defaultProvider = DefaultDateTimeProvider.Instance;
        }

        /// <summary>
        /// Overrides <see cref="Current"/> for the current asynchronous flow, and anything started from it,
        /// until the returned scope is disposed. Concurrent flows are unaffected, so this is safe to use
        /// from tests running in parallel.
        /// </summary>
        /// <param name="provider">The provider to use for the current asynchronous flow.</param>
        /// <returns>A scope which restores the previously scoped provider when disposed.</returns>
        public static IDisposable UseProvider(IDateTimeProvider provider)
        {
            CheckProvider(nameof(provider), provider);

            var scope = new ProviderScope(_scopedProvider.Value);
            _scopedProvider.Value = provider;

            return scope;
        }

        private static void CheckProvider(string paramName, IDateTimeProvider provider)
        {
            ArgCheck.NotNull(paramName, provider);

            if (ReferenceEquals(provider, CurrentDateTimeProvider.Instance))
            {
                // It forwards back to Current, which would recurse until the stack ran out.
                throw new ArgumentException($"Parameter {paramName} cannot be the current date time provider itself.", paramName);
            }
        }

        private sealed class ProviderScope : IDisposable
        {
            private readonly IDateTimeProvider? _previousProvider;
            private bool _disposed;

            public ProviderScope(IDateTimeProvider? previousProvider)
            {
                _previousProvider = previousProvider;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _scopedProvider.Value = _previousProvider;
                }
            }
        }
    }
}

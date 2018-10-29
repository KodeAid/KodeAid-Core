// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid
{
    public class DateTimeProvider
    {
        public static IDateTimeProvider Current { get; private set; } = DefaultDateTimeProvider.Instance;

        public static void SetCurrentProvider(IDateTimeProvider provider)
        {
            ArgCheck.NotNull(nameof(provider), provider);
            Current = provider;
        }

        public static void ResetCurrentProviderToDefault()
        {
            Current = DefaultDateTimeProvider.Instance;
        }
    }
}

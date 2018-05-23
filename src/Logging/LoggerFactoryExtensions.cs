// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static ILogger CreateLoggerOrNopIfNull(this ILoggerFactory loggerFactory, string categoryName)
        {
            return loggerFactory?.CreateLogger(categoryName) ?? NopLogger.Instance;
        }

        public static ILogger<T> CreateLoggerOrNopIfNull<T>(this ILoggerFactory loggerFactory)
        {
            return loggerFactory?.CreateLogger<T>() ?? NopLogger<T>.Instance;
        }
    }
}

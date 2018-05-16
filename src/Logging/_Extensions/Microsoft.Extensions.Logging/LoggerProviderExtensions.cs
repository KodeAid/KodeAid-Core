// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using KodeAid.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerProviderExtensions
    {
        public static ILogger CreateLoggerOrEmpty(this ILoggerProvider loggerProvider, string categoryName)
        {
            return loggerProvider?.CreateLogger(categoryName) ?? LoggerHelper.Empty;
        }
    }
}

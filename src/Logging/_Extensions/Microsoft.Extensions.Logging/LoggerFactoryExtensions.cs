// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using KodeAid.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static ILogger CreateLoggerOrEmpty(this ILoggerFactory loggerFactory, string categoryName)
        {
            return loggerFactory?.CreateLogger(categoryName) ?? LoggerHelper.Empty;
        }

        public static ILogger CreateLoggerOrEmpty(this ILoggerFactory loggerFactory, Type type)
        {
            return loggerFactory?.CreateLogger(type) ?? LoggerHelper.Empty;
        }

        public static ILogger<TCategory> CreateLoggerOrEmpty<TCategory>(this ILoggerFactory loggerFactory)
        {
            return loggerFactory?.CreateLogger<TCategory>() ?? LoggerHelper.CreateEmptyLogger<TCategory>();
        }
    }
}

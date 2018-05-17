// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using KodeAid;
using Microsoft.Extensions.Logging;

namespace KodeAid.Logging
{
    public static class LoggerHelper
    {
        public static readonly ILogger Empty = CreateEmptyLogger<object>();

        public static ILogger<TCategory> CreateEmptyLogger<TCategory>() => EmptyLogger<TCategory>.Empty;

        private class EmptyLogger<TCategory> : ILogger<TCategory>
        {
            public static readonly ILogger<TCategory> Empty = new EmptyLogger<TCategory>();

            protected EmptyLogger()
            {
            }

            IDisposable ILogger.BeginScope<TState>(TState state) => Disposable.NoOp;

            bool ILogger.IsEnabled(LogLevel logLevel) => false;

            void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
            }
        }
    }
}

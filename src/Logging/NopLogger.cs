// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using Microsoft.Extensions.Logging;

namespace KodeAid.Logging
{
    public class NopLogger : ILogger
    {
        public static readonly ILogger Instance = new NopLogger();

        public IDisposable BeginScope<TState>(TState state) => Disposable.Nop;

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
    }

    public class NopLogger<T> : NopLogger, ILogger<T>
    {
        public static new readonly ILogger<T> Instance = new NopLogger<T>();
    }
}

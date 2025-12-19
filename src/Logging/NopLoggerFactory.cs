// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.Extensions.Logging;

namespace KodeAid.Logging
{
    public class NopLoggerFactory : ILoggerFactory
    {
        public static readonly ILoggerFactory Instance = new NopLoggerFactory();

        public ILogger CreateLogger(string categoryName) => NopLogger.Instance;

        public void AddProvider(ILoggerProvider provider) { }

        public void Dispose() { }
    }
}

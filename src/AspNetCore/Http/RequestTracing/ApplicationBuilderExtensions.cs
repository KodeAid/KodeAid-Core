// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Linq;
using KodeAid.AspNetCore.Http.RequestTracing;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// This will affect performance.
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, params string[] ignoredPathPrefixes)
        {
            return builder.UseRequestLogging(LogLevel.Trace, ignoredPathPrefixes);
        }

        /// <summary>
        /// This will affect performance.
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, LogLevel logLevel, params string[] ignoredPathPrefixes)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>(logLevel, ignoredPathPrefixes.AsEnumerable());
        }

        /// <summary>
        /// This should be called before anything else, including error handling, as it needs to create a new response stream.
        /// This will affect performance.
        /// </summary>
        public static IApplicationBuilder UseResponseLogging(this IApplicationBuilder builder, params string[] ignoredPathPrefixes)
        {
            return builder.UseResponseLogging(LogLevel.Trace, ignoredPathPrefixes);
        }

        /// <summary>
        /// This should be called before anything else, including error handling, as it needs to create a new response stream.
        /// This will affect performance.
        /// </summary>
        public static IApplicationBuilder UseResponseLogging(this IApplicationBuilder builder, LogLevel logLevel, params string[] ignoredPathPrefixes)
        {
            return builder.UseMiddleware<ResponseLoggingMiddleware>(logLevel, ignoredPathPrefixes.AsEnumerable());
        }
    }
}

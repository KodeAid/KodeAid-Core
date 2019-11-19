// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using KodeAid.AspNetCore.Http.Tracing;
using KodeAid.AspNetCore.Http.Tracing.Request;
using KodeAid.AspNetCore.Http.Tracing.Response;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// This will affect performance.
        /// </summary>
        public static IApplicationBuilder UseRequestTracing(this IApplicationBuilder builder, Action<HttpTraceOptions> setupAction = null)
        {
            var options = new HttpTraceOptions();
            setupAction?.Invoke(options);

            if (options.RequestMode != HttpTraceMode.Disabled)
            {
                builder.UseRequestLogging(options.RequestMode == HttpTraceMode.IncludeBody, options.LogLevel, options.IgnoredPathPrefixes);
            }

            if (options.ResponseMode != HttpTraceMode.Disabled)
            {
                builder.UseResponseLogging(options.ResponseMode == HttpTraceMode.IncludeBody, options.LogLevel, options.IgnoredPathPrefixes);
            }

            return builder;
        }

        /// <summary>
        /// This will affect performance.
        /// </summary>
        internal static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, bool includeBody = false, LogLevel logLevel = LogLevel.Trace, IEnumerable<string> ignoredPathPrefixes = null)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>(includeBody, ignoredPathPrefixes.AsEnumerable(), logLevel);
        }

        /// <summary>
        /// This should be called before anything else, including error handling, as it needs to create a new response stream.
        /// This will affect performance.
        /// </summary>
        internal static IApplicationBuilder UseResponseLogging(this IApplicationBuilder builder, bool includeBody = false, LogLevel logLevel = LogLevel.Trace, IEnumerable<string> ignoredPathPrefixes = null)
        {
            return builder.UseMiddleware<ResponseLoggingMiddleware>(includeBody, ignoredPathPrefixes.AsEnumerable(), logLevel);
        }
    }
}

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
        /// This should be called before anything else, including error handling, as it needs to create a new response stream if response body logging is enabled.
        /// </summary>
        public static IApplicationBuilder UseRequestTracing(this IApplicationBuilder builder, Action<HttpTraceOptions> setupAction = null)
        {
            var options = new HttpTraceOptions();
            setupAction?.Invoke(options);

            if (options.RequestMode != HttpTraceMode.Disabled)
            {
                builder.UseRequestLogging(options.RequestMode == HttpTraceMode.IncludeBody, options.IgnoredPathPrefixes, options.LogLevel);
            }

            if (options.ResponseMode != HttpTraceMode.Disabled)
            {
                builder.UseResponseLogging(options.ResponseMode == HttpTraceMode.IncludeBody, options.IgnoredPathPrefixes, options.LogLevel);
            }

            return builder;
        }

        /// <summary>
        /// This will affect performance.
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, bool includeBody = false, IEnumerable<string> ignoredPathPrefixes = null, LogLevel logLevel = LogLevel.Trace)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>(includeBody, ignoredPathPrefixes, logLevel);
        }

        /// <summary>
        /// This will affect performance.
        /// This should be called before anything else, including error handling, as it needs to create a new response stream.
        /// </summary>
        public static IApplicationBuilder UseResponseLogging(this IApplicationBuilder builder, bool includeBody = false, IEnumerable<string> ignoredPathPrefixes = null, LogLevel logLevel = LogLevel.Trace)
        {
            return builder.UseMiddleware<ResponseLoggingMiddleware>(includeBody, ignoredPathPrefixes, logLevel);
        }
    }
}

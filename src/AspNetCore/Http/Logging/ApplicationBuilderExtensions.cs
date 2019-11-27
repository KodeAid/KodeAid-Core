// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using KodeAid;
using KodeAid.AspNetCore.Http.Logging;
using KodeAid.AspNetCore.Http.Logging.Request;
using KodeAid.AspNetCore.Http.Logging.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// This will affect performance.
        /// This should be called before anything else, including error handling, as it needs to create a new response stream if response body logging is enabled.
        /// </summary>
        public static IApplicationBuilder UseHttpLogging(this IApplicationBuilder builder, Action<HttpLoggingOptions> setupAction = null)
        {
            var options = new HttpLoggingOptions();
            setupAction?.Invoke(options);

            return builder
                .UseRequestLogging(options.Request)
                .UseResponseLogging(options.Response);
        }

        /// <summary>
        /// This will affect performance.
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, Action<RequestLoggingOptions> setupAction = null)
        {
            var options = new RequestLoggingOptions();
            setupAction?.Invoke(options);

            return builder.UseRequestLogging(options);
        }

        /// <summary>
        /// This will affect performance.
        /// This should be called before anything else, including error handling, as it needs to create a new response stream.
        /// </summary>
        public static IApplicationBuilder UseResponseLogging(this IApplicationBuilder builder, Action<ResponseLoggingOptions> setupAction = null)
        {
            var options = new ResponseLoggingOptions();
            setupAction?.Invoke(options);

            return builder.UseResponseLogging(options);
        }

        /// <summary>
        /// This will affect performance.
        /// </summary>
        private static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder, RequestLoggingOptions options)
        {
            if (options == null)
            {
                options = new RequestLoggingOptions();
            }

            if (options.Enabled)
            {
                var logger = builder.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(options.LoggerName ?? typeof(RequestLoggingMiddleware).Namespace);
                builder.UseMiddleware<RequestLoggingMiddleware>(logger, options.MaxBodyByteCount, options.ShouldLog);
            }

            return builder;
        }

        /// <summary>
        /// This will affect performance.
        /// This should be called before anything else, including error handling, as it needs to create a new response stream.
        /// </summary>
        private static IApplicationBuilder UseResponseLogging(this IApplicationBuilder builder, ResponseLoggingOptions options)
        {
            if (options == null)
            {
                options = new ResponseLoggingOptions();
            }

            if (options.Enabled)
            {
                var logger = builder.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(options.LoggerName ?? typeof(ResponseLoggingMiddleware).Namespace);
                builder.UseMiddleware<ResponseLoggingMiddleware>(logger, options.MaxBodyByteCount, options.ShouldLog);
            }

            return builder;
        }
    }
}

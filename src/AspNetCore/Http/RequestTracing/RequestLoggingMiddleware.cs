// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KodeAid.AspNetCore.Http.RequestTracing
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LogLevel _logLevel;
        private readonly IEnumerable<string> _ignoredPathPrefixes;

        public RequestLoggingMiddleware(RequestDelegate next, LogLevel logLevel, IEnumerable<string> ignoredPathPrefixes)
        {
            _next = next;
            _logLevel = logLevel;
            _ignoredPathPrefixes = ignoredPathPrefixes?.EmptyIfNull().WhereNotNull().ToList();
        }

        public async Task Invoke(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
        {
            if (!logger.IsEnabled(_logLevel) || _ignoredPathPrefixes.Any(p => context.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            logger.Log(_logLevel, await FormatRequest(context.Request));
            await _next(context);
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var queryAsText = string.Join("\n", request.Query.Select(q => $"{q.Key}={q.Value.FirstOrDefault()}"));
            var headersAsText = string.Join("\n", request.Headers.Select(h => $"{h.Key}: {h.Value.FirstOrDefault()}"));
            //var body = request.Body;
            request.EnableBuffering();
            var buffer = new byte[Math.Min(Math.Max(request.Body.Length, request.ContentLength.GetValueOrDefault()), int.MaxValue)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;
            //request.Body = request.Body;
            return $"REQUEST TRACE: {request.Scheme} {request.Host}{request.Path}\n{headersAsText}\n{queryAsText}\n{bodyAsText}";
        }
    }
}

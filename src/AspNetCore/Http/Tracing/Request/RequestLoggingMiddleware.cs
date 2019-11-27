// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;

namespace KodeAid.AspNetCore.Http.Tracing.Request
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly long _maxBodyByteCount;
        private readonly IEnumerable<string> _ignoredPathPrefixes;
        private readonly LogLevel _logLevel;

        public RequestLoggingMiddleware(RequestDelegate next, long maxBodyByteCount, IEnumerable<string> ignoredPathPrefixes, LogLevel logLevel)
        {
            ArgCheck.NotNull(nameof(next), next);

            _next = next;
            _maxBodyByteCount = maxBodyByteCount;
            _ignoredPathPrefixes = ignoredPathPrefixes?.EmptyIfNull().WhereNotNull().ToList();
            _logLevel = logLevel;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
        {
            if (logger.IsEnabled(_logLevel) && !_ignoredPathPrefixes.Any(p => context.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
            {
                logger.Log(_logLevel, await FormatRequestAsync(context.Request));
            }

            await _next(context);
        }

        private async Task<string> FormatRequestAsync(HttpRequest request)
        {
            var headersAsText = string.Join("\n", request.Headers.Select(h => $"{h.Key}: {h.Value}"));
            var queryAsText = string.Join("\n", request.Query.Select(q => $"{q.Key}={q.Value}"));

            if (_maxBodyByteCount <= 0)
            {
                return $"REQUEST TRACE: {request.Method.ToString().ToUpper()} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}\n{queryAsText}";
            }

            request.EnableRewind();
            var buffer = new byte[Math.Min(Math.Max(request.Body.Length, request.ContentLength.GetValueOrDefault()), _maxBodyByteCount)];
            var read = await request.Body.ReadAsync(buffer, 0, buffer.Length);
            request.Body.Position = 0;

            var bodyAsText = Encoding.UTF8.GetString(buffer, 0, read);

            return $"REQUEST TRACE: {request.Method.ToString().ToUpper()} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}\n{queryAsText}\n{bodyAsText}";
        }
    }
}

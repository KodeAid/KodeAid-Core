// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KodeAid.AspNetCore.Http.Logging.Request
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly long _maxBodyByteCount;
        private readonly Func<HttpContext, bool> _shouldLog;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger logger, long maxBodyByteCount, Func<HttpContext, bool> shouldLog)
        {
            ArgCheck.NotNull(nameof(next), next);
            ArgCheck.NotNull(nameof(logger), logger);

            _next = next;
            _logger = logger;
            _maxBodyByteCount = maxBodyByteCount;
            _shouldLog = shouldLog;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_logger.IsEnabled(LogLevel.Trace) &&
                (_shouldLog?.Invoke(context)).GetValueOrDefault(true))
            {
                _logger.LogTrace(await FormatRequestAsync(context.Request));
            }

            await _next(context);
        }

        private async Task<string> FormatRequestAsync(HttpRequest request)
        {
            var headersAsText = string.Join("", request.Headers.Select(h => $"{h.Key}: {h.Value}\n"));
            var queryAsText = string.Join("", request.Query.Select((q, i) => $"{(i == 0 ? "?" : "&")}{q.Key}={q.Value}\n"));

            if (_maxBodyByteCount <= 0)
            {
                return $"REQUEST TRACE: {request.Method.ToString().ToUpper()} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}{queryAsText}";
            }

            request.EnableBuffering();
            var buffer = new byte[Math.Min(Math.Max(request.Body.Length, request.ContentLength.GetValueOrDefault()), _maxBodyByteCount)];
            var read = await request.Body.ReadAsync(buffer, 0, buffer.Length);
            request.Body.Position = 0;

            var bodyAsText = Encoding.UTF8.GetString(buffer, 0, read);

            return $"REQUEST TRACE: {request.Method.ToString().ToUpper()} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}{queryAsText}\n{bodyAsText}";
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KodeAid.AspNetCore.Http.Tracing.Response
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly long _maxBodyByteCount;
        private readonly IEnumerable<string> _ignoredPathPrefixes;
        private readonly LogLevel _logLevel;

        public ResponseLoggingMiddleware(RequestDelegate next, long maxBodyByteCount, IEnumerable<string> ignoredPathPrefixes, LogLevel logLevel)
        {
            ArgCheck.NotNull(nameof(next), next);

            _next = next;
            _maxBodyByteCount = maxBodyByteCount;
            _ignoredPathPrefixes = ignoredPathPrefixes?.EmptyIfNull().WhereNotNull().ToList();
            _logLevel = logLevel;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<ResponseLoggingMiddleware> logger)
        {
            if (!logger.IsEnabled(_logLevel) || _ignoredPathPrefixes.Any(p => context.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            await _next(context);
            logger.Log(_logLevel, await FormatResponseAsync(context.Request, context.Response));

            try
            {
                if (context.Response.StatusCode != 204 && context.Response.StatusCode != 205 && context.Response.StatusCode != 304)
                {
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to copy logged response body back to original stream, response status code is {StatusCode}.", context.Response.StatusCode);
            }
        }

        private async Task<string> FormatResponseAsync(HttpRequest request, HttpResponse response)
        {
            var headersAsText = string.Join("\n", response.Headers.Select(h => $"{h.Key}: {h.Value}"));

            if (_maxBodyByteCount <= 0)
            {
                return $"RESPONSE TRACE: {response.StatusCode} {((HttpStatusCode)response.StatusCode).ToString()} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}";
            }

            response.Body.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[Math.Min(Math.Max(response.Body.Length, response.ContentLength.GetValueOrDefault()), _maxBodyByteCount)];
            var read = await response.Body.ReadAsync(buffer, 0, buffer.Length);
            response.Body.Seek(0, SeekOrigin.Begin);

            var bodyAsText = Encoding.UTF8.GetString(buffer, 0, read);

            return $"RESPONSE TRACE: {response.StatusCode}/{((HttpStatusCode)response.StatusCode).ToString()} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}\n{bodyAsText}";
        }
    }
}

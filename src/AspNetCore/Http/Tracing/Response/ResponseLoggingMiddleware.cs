// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KodeAid.AspNetCore.Http.Tracing.Response
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _includeBody;
        private readonly IEnumerable<string> _ignoredPathPrefixes;
        private readonly LogLevel _logLevel;

        public ResponseLoggingMiddleware(RequestDelegate next, bool includeBody, IEnumerable<string> ignoredPathPrefixes, LogLevel logLevel)
        {
            ArgCheck.NotNull(nameof(next), next);

            _next = next;
            _includeBody = includeBody;
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
            logger.Log(_logLevel, await FormatResponseAsync(context.Request, context.Response, _includeBody));

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

        private async Task<string> FormatResponseAsync(HttpRequest request, HttpResponse response, bool includeBody)
        {
            var headersAsText = string.Join("\n", response.Headers.Select(h => $"{h.Key}: {h.Value}"));

            if (!includeBody)
            {
                return $"RESPONSE TRACE: {response.StatusCode} {((HttpStatusCode)response.StatusCode).ToString()} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}";
            }

            response.Body.Seek(0, SeekOrigin.Begin);
#pragma warning disable IDE0067 // Dispose objects before losing scope
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
#pragma warning restore IDE0067 // Dispose objects before losing scope
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"RESPONSE TRACE: {response.StatusCode}/{((HttpStatusCode)response.StatusCode).ToString()} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}\n{bodyAsText}";
        }
    }
}

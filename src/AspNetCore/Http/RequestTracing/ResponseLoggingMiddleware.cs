// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KodeAid.AspNetCore.Http.RequestTracing
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<string> _ignoredPathPrefixes;

        public ResponseLoggingMiddleware(RequestDelegate next, IEnumerable<string> ignoredPathPrefixes)
        {
            _next = next;
            _ignoredPathPrefixes = ignoredPathPrefixes?.EmptyIfNull().WhereNotNull().ToList();
        }

        public async Task Invoke(HttpContext context, ILogger<ResponseLoggingMiddleware> logger)
        {
            if (_ignoredPathPrefixes.Any(p => context.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                await _next(context);
                logger.LogTrace(await FormatResponse(context.Request, context.Response));

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
        }

        private async Task<string> FormatResponse(HttpRequest request, HttpResponse response)
        {
            var headersAsText = string.Join("\n", response.Headers.Select(h => $"{h.Key}: {h.Value.FirstOrDefault()}"));
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"RESPONSE TRACE: {request.Scheme} {request.Host}{request.Path}\n{headersAsText}\n{bodyAsText}";
        }
    }
}

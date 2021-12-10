// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KodeAid.AspNetCore.Http.Logging.Response
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _maxBodyByteCount;
        private readonly Func<HttpContext, bool> _shouldLog;
        private readonly ILogger _logger;
        private readonly string _prefix;

        public ResponseLoggingMiddleware(RequestDelegate next, ILogger logger, int maxBodyByteCount, Func<HttpContext, bool> shouldLog, string prefix)
        {
            ArgCheck.NotNull(nameof(next), next);
            ArgCheck.NotNull(nameof(logger), logger);

            _next = next;
            _logger = logger;
            _maxBodyByteCount = maxBodyByteCount;
            _shouldLog = shouldLog;
            _prefix = prefix ?? "RESPONSE TRACE: ";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_logger.IsEnabled(LogLevel.Trace) ||
                !(_shouldLog?.Invoke(context)).GetValueOrDefault(true))
            {
                await _next(context);
                return;
            }

            if (_maxBodyByteCount <= 0)
            {
                await _next(context);
                _logger.LogTrace(await FormatResponseAsync(context.Request, context.Response));
                return;
            }

            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();

            context.Response.Body = responseBody;

            await _next(context);

            _logger.LogTrace(await FormatResponseAsync(context.Request, context.Response));

            try
            {
                if (responseBody.Length > 0 && context.Response.StatusCode != 204 && context.Response.StatusCode != 205 && context.Response.StatusCode != 304)
                {
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to copy logged response body back to original stream, response status code is {StatusCode}.", context.Response.StatusCode);
            }
        }

        private async Task<string> FormatResponseAsync(HttpRequest request, HttpResponse response)
        {
            var headersAsText = string.Join("", response.Headers.Select(h => $"{h.Key}: {h.Value}\n"));

            if (_maxBodyByteCount <= 0)
            {
                return $"{_prefix}{response.StatusCode} {(HttpStatusCode)response.StatusCode} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}";
            }

            response.Body.Seek(0, SeekOrigin.Begin);
            var buffer = ArrayPool<byte>.Shared.Rent((int)Math.Min(Math.Max(response.Body.Length, response.ContentLength.GetValueOrDefault()), _maxBodyByteCount));

            try
            {
                var read = await response.Body.ReadAsync(buffer, 0, buffer.Length);
                response.Body.Seek(0, SeekOrigin.Begin);
                var bodyAsText = Encoding.UTF8.GetString(buffer, 0, read);

                return $"{_prefix}{response.StatusCode}/{(HttpStatusCode)response.StatusCode} {request.Scheme.ToLower()}://{request.Host.ToString().ToLower()}{request.Path.ToString().ToLower()}\n{headersAsText}\n{bodyAsText}";
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}

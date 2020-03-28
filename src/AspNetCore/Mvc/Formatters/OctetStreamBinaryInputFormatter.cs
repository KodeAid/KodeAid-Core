// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;

namespace KodeAid.AspNetCore.Mvc.Formatters
{
    public class OctetStreamBinaryInputFormatter : InputFormatter
    {
        private readonly bool _suppressInputFormatterBuffering;

        public OctetStreamBinaryInputFormatter()
            : this(suppressInputFormatterBuffering: false)
        {
        }

        public OctetStreamBinaryInputFormatter(bool suppressInputFormatterBuffering)
        {
            _suppressInputFormatterBuffering = suppressInputFormatterBuffering;

            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationOctetStream);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Body.CanSeek && !_suppressInputFormatterBuffering)
            {
                request.EnableBuffering();
                Debug.Assert(request.Body.CanSeek);
                await request.Body.DrainAsync(CancellationToken.None);
                request.Body.Seek(0L, SeekOrigin.Begin);
            }

            using var ms = new MemoryStream(2048);
            await request.Body.CopyToAsync(ms);
            var buffer = ms.ToArray();
            return await InputFormatterResult.SuccessAsync(buffer);
        }
    }
}

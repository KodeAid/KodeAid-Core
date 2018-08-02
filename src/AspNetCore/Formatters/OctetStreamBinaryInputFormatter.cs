// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace KodeAid.AspNetCore.Formatters
{
    public class OctetStreamBinaryInputFormatter : InputFormatter
    {
        private const string _octetStreamMediaType = "application/octet-stream";

        public OctetStreamBinaryInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(_octetStreamMediaType));
        }

        public override bool CanRead(InputFormatterContext context)
        {
            ArgCheck.NotNull(nameof(context), context);

            var contentType = context.HttpContext.Request.ContentType;
            if (contentType == _octetStreamMediaType)
                return true;

            return false;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;

            if (contentType == _octetStreamMediaType)
            {
                using (var ms = new MemoryStream(2048))
                {
                    await request.Body.CopyToAsync(ms);
                    var content = ms.ToArray();
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }

            return await InputFormatterResult.FailureAsync();
        }
    }
}

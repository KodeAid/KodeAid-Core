// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace KodeAid.AspNetCore.Formatters
{
    public class PlainTextStringInputFormatter : InputFormatter
    {
        private const string _plainTextMediaType = "text/plain";

        public PlainTextStringInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(_plainTextMediaType));
        }

        public override bool CanRead(InputFormatterContext context)
        {
            ArgCheck.NotNull(nameof(context), context);

            var contentType = context.HttpContext.Request.ContentType;
            if (contentType == _plainTextMediaType)
                return true;

            return false;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;

            if (contentType == _plainTextMediaType)
            {
                using (var reader = new StreamReader(request.Body))
                {
                    var content = await reader.ReadToEndAsync();
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }

            return await InputFormatterResult.FailureAsync();
        }
    }
}

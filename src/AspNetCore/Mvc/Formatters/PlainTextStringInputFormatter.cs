// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;

namespace KodeAid.AspNetCore.Mvc.Formatters
{
    public class PlainTextStringInputFormatter : TextInputFormatter
    {
        private readonly bool _suppressInputFormatterBuffering;

        public PlainTextStringInputFormatter()
            : this(suppressInputFormatterBuffering: false)
        {
        }

        public PlainTextStringInputFormatter(bool suppressInputFormatterBuffering)
        {
            _suppressInputFormatterBuffering = suppressInputFormatterBuffering;

            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);

            SupportedMediaTypes.Add(MediaTypeHeaderValues.TextPlain);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            ArgCheck.NotNull(nameof(context), context);
            ArgCheck.NotNull(nameof(encoding), encoding);

            var request = context.HttpContext.Request;

            if (!request.Body.CanSeek && !_suppressInputFormatterBuffering)
            {
                BufferingHelper.EnableRewind(request);
                Debug.Assert(request.Body.CanSeek);

                await request.Body.DrainAsync(CancellationToken.None);
                request.Body.Seek(0L, SeekOrigin.Begin);
            }

            using (var reader = new StreamReader(request.Body))
            {
                var text = await reader.ReadToEndAsync();
                return await InputFormatterResult.SuccessAsync(text);
            }
        }
    }
}

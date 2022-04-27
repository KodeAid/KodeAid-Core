// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;

namespace KodeAid.AspNetCore.Mvc.Formatters
{
    public class FormDictionaryInputFormatter : TextInputFormatter
    {
        private readonly bool _suppressInputFormatterBuffering;

        public FormDictionaryInputFormatter()
            : this(suppressInputFormatterBuffering: false)
        {
        }

        public FormDictionaryInputFormatter(bool suppressInputFormatterBuffering)
        {
            _suppressInputFormatterBuffering = suppressInputFormatterBuffering;

            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);

            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationFormUrlEncoded);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            ArgCheck.NotNull(nameof(context), context);
            ArgCheck.NotNull(nameof(encoding), encoding);

            var request = context.HttpContext.Request;

            if (!request.Body.CanSeek && !_suppressInputFormatterBuffering)
            {
                request.EnableBuffering();
                Debug.Assert(request.Body.CanSeek);
                await request.Body.DrainAsync(CancellationToken.None);
                request.Body.Seek(0L, SeekOrigin.Begin);
            }

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            var form = body.Split('&').Select(x => x.Split('=')).ToDictionary(key => key[0].Trim(), value => Uri.UnescapeDataString(value.Skip(1).FirstOrDefault()?.Trim() ?? string.Empty));
            return await InputFormatterResult.SuccessAsync(form);
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KodeAid.AspNetCore.Mvc.Formatters.Internal
{
    /// <summary>
    /// A <see cref="IConfigureOptions{TOptions}"/> implementation which will add the
    /// text formatters to <see cref="MvcOptions"/>.
    /// </summary>
    public class PlainTextStringMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="PlainTextStringMvcOptionsSetup"/>.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public PlainTextStringMvcOptionsSetup(ILoggerFactory loggerFactory)
        {
            ArgCheck.NotNull(nameof(loggerFactory), loggerFactory);

            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Adds the XML serializer formatters to <see cref="MvcOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/>.</param>
        public void Configure(MvcOptions options)
        {
            // Do not override any user mapping
            var key = "text";
            var mapping = options.FormatterMappings.GetMediaTypeMappingForFormat(key);
            if (string.IsNullOrEmpty(mapping))
            {
                options.FormatterMappings.SetMediaTypeMappingForFormat(
                    key,
                    MediaTypeHeaderValues.TextPlain);
            }

            options.InputFormatters.Add(new PlainTextStringInputFormatter(options.SuppressInputFormatterBuffering));
        }
    }
}

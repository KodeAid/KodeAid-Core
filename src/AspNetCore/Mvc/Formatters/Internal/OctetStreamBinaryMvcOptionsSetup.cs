// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KodeAid.AspNetCore.Mvc.Formatters.Internal
{
    /// <summary>
    /// A <see cref="IConfigureOptions{TOptions}"/> implementation which will add the
    /// binary formatters to <see cref="MvcOptions"/>.
    /// </summary>
    public class OctetStreamBinaryMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="OctetStreamBinaryMvcOptionsSetup"/>.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public OctetStreamBinaryMvcOptionsSetup(ILoggerFactory loggerFactory)
        {
            ArgCheck.NotNull(nameof(loggerFactory), loggerFactory);

            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Adds the binary formatters to <see cref="MvcOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/>.</param>
        public void Configure(MvcOptions options)
        {
            // Do not override any user mapping
            var key = "octet-stream";
            var mapping = options.FormatterMappings.GetMediaTypeMappingForFormat(key);
            if (string.IsNullOrEmpty(mapping))
            {
                options.FormatterMappings.SetMediaTypeMappingForFormat(
                    key,
                    MediaTypeHeaderValues.ApplicationOctetStream);
            }

            options.InputFormatters.Add(new OctetStreamBinaryInputFormatter(options.SuppressInputFormatterBuffering));
        }
    }
}

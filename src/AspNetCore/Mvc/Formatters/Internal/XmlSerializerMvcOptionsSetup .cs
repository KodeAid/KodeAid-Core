// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KodeAid.AspNetCore.Mvc.Formatters.Internal
{
    /// <summary>
    /// A <see cref="IConfigureOptions{TOptions}"/> implementation which will add the
    /// XML serializer formatters to <see cref="MvcOptions"/>.
    /// </summary>
    public class XmlSerializerMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly XmlSerializerFormatterOptions _options;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="XmlSerializerMvcOptionsSetup "/>.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public XmlSerializerMvcOptionsSetup(XmlSerializerFormatterOptions options, ILoggerFactory loggerFactory)
        {
            ArgCheck.NotNull(nameof(loggerFactory), loggerFactory);

            _options = options;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Adds the XML serializer formatters to <see cref="MvcOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/>.</param>
        public void Configure(MvcOptions options)
        {
            // Do not override any user mapping
            var key = "xml";
            var mapping = options.FormatterMappings.GetMediaTypeMappingForFormat(key);
            if (string.IsNullOrEmpty(mapping))
            {
                options.FormatterMappings.SetMediaTypeMappingForFormat(
                    key,
                    MediaTypeHeaderValues.ApplicationXml);
            }

            options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            options.InputFormatters.Add(new XmlSerializerInputFormatter(_options, options));
        }
    }
}

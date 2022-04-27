// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;
using KodeAid.AspNetCore.Mvc.Formatters;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for addingm formatters to MVC.
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds the binary formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddBinaryFormatters(this IMvcBuilder builder)
        {
            ArgCheck.NotNull(nameof(builder), builder);

            MvcCoreBuilderExtensions.AddBinaryFormatterServices(builder.Services);
            return builder;
        }

        /// <summary>
        /// Adds the form dictionary formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddFormDictionaryFormatters(this IMvcBuilder builder)
        {
            ArgCheck.NotNull(nameof(builder), builder);

            MvcCoreBuilderExtensions.AddFormDictionaryFormatterServices(builder.Services);
            return builder;
        }

        /// <summary>
        /// Adds the text formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddTextFormatters(this IMvcBuilder builder)
        {
            ArgCheck.NotNull(nameof(builder), builder);

            MvcCoreBuilderExtensions.AddTextFormatterServices(builder.Services);
            return builder;
        }

        /// <summary>
        /// Adds the XML Serializer formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddXmlSerializerFormatters(this IMvcBuilder builder, XmlSerializerFormatterOptions options)
        {
            ArgCheck.NotNull(nameof(builder), builder);

            MvcCoreBuilderExtensions.AddXmlSerializerFormatterServices(builder.Services, options);
            return builder;
        }
    }
}

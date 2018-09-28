// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



using KodeAid;
using KodeAid.AspNetCore.Mvc.Formatters;
using KodeAid.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding formatters to MVC.
    /// </summary>
    public static class MvcCoreBuilderExtensions
    {
        /// <summary>
        /// Adds the binary formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <returns>The <see cref="IMvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder AddBinaryFormatters(this IMvcCoreBuilder builder)
        {
            ArgCheck.NotNull(nameof(builder), builder);

            AddBinaryFormatterServices(builder.Services);
            return builder;
        }

        /// <summary>
        /// Adds the text formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <returns>The <see cref="IMvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder AddTextFormatters(this IMvcCoreBuilder builder)
        {
            ArgCheck.NotNull(nameof(builder), builder);

            AddTextFormatterServices(builder.Services);
            return builder;
        }

        /// <summary>
        /// Adds the XML Serializer formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <returns>The <see cref="IMvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder AddXmlSerializerFormatters(this IMvcCoreBuilder builder, XmlSerializerFormatterOptions options)
        {
            ArgCheck.NotNull(nameof(builder), builder);

            AddXmlSerializerFormatterServices(builder.Services, options);
            return builder;
        }

        // Internal for testing.
        internal static void AddBinaryFormatterServices(IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, OctetStreamBinaryMvcOptionsSetup>());
        }

        // Internal for testing.
        internal static void AddTextFormatterServices(IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, PlainTextStringMvcOptionsSetup>());
        }

        // Internal for testing.
        internal static void AddXmlSerializerFormatterServices(IServiceCollection services, XmlSerializerFormatterOptions options)
        {
            services.AddTransient(sp => new XmlSerializerMvcOptionsSetup(options, sp.GetRequiredService<ILoggerFactory>()));

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, XmlSerializerMvcOptionsSetup>());
        }
    }
}
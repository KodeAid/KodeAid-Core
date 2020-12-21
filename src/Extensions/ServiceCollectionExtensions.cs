// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using KodeAid;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCurrentDateTimeProvider(this IServiceCollection services)
            => services.AddSingleton(DateTimeProvider.Current);

        public static IServiceCollection AddDefaultDateTimeProvider(this IServiceCollection services)
            => services.AddSingleton(DefaultDateTimeProvider.Instance);

        public static IServiceCollection AddTransient<TService, TImplementation, TOptions>(this IServiceCollection services, TOptions defaultOptions = default, string configurationSection = default, Action<IServiceProvider, TOptions> verifyOptions = default)
            where TService : class
            where TImplementation : TService
        {
            ArgCheck.NotNull(nameof(services), services);

            return services.AddTransient(CreateImplementationFactory<TService, TImplementation, TOptions>(defaultOptions, configurationSection, verifyOptions));
        }

        public static IServiceCollection AddTransient<TService, TImplementation, TOptions, TOptionsBuilder>(this IServiceCollection services, Action<TOptionsBuilder> defaultSetup = default, Action<TOptionsBuilder> setup = default, string configurationSection = default, Action<IServiceProvider, TOptions> verifyOptions = default)
            where TService : class
            where TImplementation : TService
            where TOptionsBuilder : IBuilder<TOptions>
        {
            ArgCheck.NotNull(nameof(services), services);

            return services.AddTransient(CreateImplementationFactory<TService, TImplementation, TOptions, TOptionsBuilder>(defaultSetup, setup, configurationSection, verifyOptions));
        }

        public static IServiceCollection AddScoped<TService, TImplementation, TOptions>(this IServiceCollection services, TOptions defaultOptions = default, string configurationSection = default, Action<IServiceProvider, TOptions> verifyOptions = default)
            where TService : class
            where TImplementation : TService
        {
            ArgCheck.NotNull(nameof(services), services);

            return services.AddScoped(CreateImplementationFactory<TService, TImplementation, TOptions>(defaultOptions, configurationSection, verifyOptions));
        }

        public static IServiceCollection AddScoped<TService, TImplementation, TOptions, TOptionsBuilder>(this IServiceCollection services, Action<TOptionsBuilder> defaultSetup = default, Action<TOptionsBuilder> setup = default, string configurationSection = default, Action<IServiceProvider, TOptions> verifyOptions = default)
            where TService : class
            where TImplementation : TService
            where TOptionsBuilder : IBuilder<TOptions>
        {
            ArgCheck.NotNull(nameof(services), services);

            return services.AddScoped(CreateImplementationFactory<TService, TImplementation, TOptions, TOptionsBuilder>(defaultSetup, setup, configurationSection, verifyOptions));
        }

        private static Func<IServiceProvider, TService> CreateImplementationFactory<TService, TImplementation, TOptions>(TOptions defaultOptions = default, string configurationSection = default, Action<IServiceProvider, TOptions> verifyOptions = default)
            where TService : class
            where TImplementation : TService
        {
            return provider =>
            {
                var options = defaultOptions;

                if (options == null)
                {
                    options = ActivatorUtilities.GetServiceOrCreateInstance<TOptions>(provider);
                }

                if (configurationSection != null)
                {
                    var configuration = provider.GetRequiredService<IConfiguration>()?.GetSection(configurationSection)
                        ?? throw new InvalidOperationException($"Configuration section '{configurationSection}' not found.");
                    configuration.Bind(options);
                }

                verifyOptions?.Invoke(provider, options);

                return ActivatorUtilities.CreateInstance<TImplementation>(provider, options);
            };
        }

        private static Func<IServiceProvider, TService> CreateImplementationFactory<TService, TImplementation, TOptions, TOptionsBuilder>(Action<TOptionsBuilder> defaultSetup = default, Action<TOptionsBuilder> setup = default, string configurationSection = default, Action<IServiceProvider, TOptions> verifyOptions = default)
            where TService : class
            where TImplementation : TService
            where TOptionsBuilder : IBuilder<TOptions>
        {
            return provider =>
            {
                var options = default(TOptions);

                if (defaultSetup != null || setup != null)
                {
                    var builder = ActivatorUtilities.GetServiceOrCreateInstance<TOptionsBuilder>(provider);
                    defaultSetup?.Invoke(builder);
                    setup?.Invoke(builder);
                    options = builder.Build();
                }

                if (options == null)
                {
                    options = ActivatorUtilities.GetServiceOrCreateInstance<TOptions>(provider);
                }

                if (configurationSection != null)
                {
                    var configuration = provider.GetRequiredService<IConfiguration>()?.GetSection(configurationSection)
                        ?? throw new InvalidOperationException($"Configuration section '{configurationSection}' not found.");
                    configuration.Bind(options);
                }

                verifyOptions?.Invoke(provider, options);

                return ActivatorUtilities.CreateInstance<TImplementation>(provider, options);
            };
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection serviceCollection,
            string environment = null, string basePath = null,
            bool isJsonFileRequired = false, string jsonFileName = "appsettings.json")
        {
            return AddConfiguration(serviceCollection, out var configuration, environment, basePath, isJsonFileRequired, jsonFileName);
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection serviceCollection,
            out IConfiguration configuration,
            string environment = null, string basePath = null,
            bool isJsonFileRequired = false, string jsonFileName = "appsettings.json")
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath ?? Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFileName, optional: !isJsonFileRequired, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (!string.IsNullOrEmpty(environment))
            {
                var fileName = Path.GetFileNameWithoutExtension(jsonFileName);
                var extension = Path.GetExtension(jsonFileName);
                builder.AddJsonFile($"{fileName}.{environment}{extension}", optional: true, reloadOnChange: true);
            }

            configuration = builder.Build();
            return serviceCollection.AddSingleton(configuration);
        }
    }
}

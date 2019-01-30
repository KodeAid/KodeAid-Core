// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using KodeAid;
using KodeAid.Azure.Storage;
using KodeAid.Security.Cryptography.X509Certificates;
using KodeAid.Security.Secrets;
using KodeAid.Storage;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureBlobStorageKeyValueStoreServiceCollectionExtensions
    {
        public static void AddAzureBlobStorageKeyValueStore(this IServiceCollection services, string storeConfigurationKey = nameof(AzureBlobStorageKeyValueStore))
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNullOrEmpty(nameof(storeConfigurationKey), storeConfigurationKey);

            services.AddTransient<IKeyValueReadOnlyStore>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>()?.GetSection(storeConfigurationKey)
                    ?? throw new InvalidOperationException($"Azure blob store configuration section '{storeConfigurationKey}' not found.");
                var options = new AzureBlobStorageKeyValueStoreOptions();
                configuration.Bind(options);

                if (options.SecretStore == null)
                {
                    options.SecretStore = serviceProvider.GetService<ISecretReadOnlyStore>();
                }

                return new AzureBlobStorageKeyValueStore(options);
            });

            services.AddTransient<IKeyValueStore>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>()?.GetSection(storeConfigurationKey)
                    ?? throw new InvalidOperationException($"Azure blob store configuration section '{storeConfigurationKey}' not found.");
                var options = new AzureBlobStorageKeyValueStoreOptions();
                configuration.Bind(options);

                if (options.SecretStore == null)
                {
                    options.SecretStore = serviceProvider.GetService<ISecretReadOnlyStore>();
                }

                return new AzureBlobStorageKeyValueStore(options);
            });
        }

        public static void AddAzureBlobStorageKeyValueStore(this IServiceCollection services, Action<AzureBlobStorageKeyValueStoreOptionsBuilder> setupAction)
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNull(nameof(setupAction), setupAction);

            services.AddTransient<IKeyValueReadOnlyStore>(serviceProvider =>
            {
                var builder = new AzureBlobStorageKeyValueStoreOptionsBuilder();
                setupAction(builder);
                var options = builder.Build();

                if (options.SecretStore == null)
                {
                    options.SecretStore = serviceProvider.GetService<ISecretReadOnlyStore>();
                }

                return new AzureBlobStorageKeyValueStore(options);
            });

            services.AddTransient<IKeyValueStore>(serviceProvider =>
            {
                var builder = new AzureBlobStorageKeyValueStoreOptionsBuilder();
                setupAction(builder);
                var options = builder.Build();

                if (options.SecretStore == null)
                {
                    options.SecretStore = serviceProvider.GetService<ISecretReadOnlyStore>();
                }

                return new AzureBlobStorageKeyValueStore(options);
            });
        }

        public static void AddAzureBlobStoragePublicCertificateStore(this IServiceCollection services, string storeConfigurationKey = "AzureBlobStoragePublicCertificateStore")
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNullOrEmpty(nameof(storeConfigurationKey), storeConfigurationKey);

            services.AddTransient<IPublicCertificateStore>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>()?.GetSection(storeConfigurationKey)
                    ?? throw new InvalidOperationException($"Azure blob store configuration section '{storeConfigurationKey}' not found.");
                var options = new AzureBlobStorageKeyValueStoreOptions();
                configuration.Bind(options);

                if (options.SecretStore == null)
                {
                    options.SecretStore = serviceProvider.GetService<ISecretReadOnlyStore>();
                }

                return new AzureBlobStorageKeyValueStore(options);
            });
        }

        public static void AddAzureBlobStoragePublicCertificateStore(this IServiceCollection services, Action<AzureBlobStorageKeyValueStoreOptionsBuilder> setupAction)
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNull(nameof(setupAction), setupAction);

            services.AddTransient<IPublicCertificateStore>(serviceProvider =>
            {
                var builder = new AzureBlobStorageKeyValueStoreOptionsBuilder();
                setupAction(builder);
                var options = builder.Build();

                if (options.SecretStore == null)
                {
                    options.SecretStore = serviceProvider.GetService<ISecretReadOnlyStore>();
                }

                return new AzureBlobStorageKeyValueStore(options);
            });
        }
    }
}

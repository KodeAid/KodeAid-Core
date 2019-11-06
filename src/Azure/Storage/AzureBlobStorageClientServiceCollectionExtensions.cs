// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using KodeAid;
using KodeAid.Azure.Storage;
using KodeAid.Security.Cryptography.X509Certificates;
using KodeAid.Security.Secrets;
using KodeAid.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureBlobStorageClientServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureBlobStorageKeyValueStore(this IServiceCollection services, string configurationSection = nameof(AzureBlobStorageClient))
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNullOrEmpty(nameof(configurationSection), configurationSection);

            return services
                .AddTransient<IKeyValueReadOnlyStore, AzureBlobStorageClient, AzureBlobStorageClientOptions>(configurationSection: configurationSection, verifyOptions: (provider, options) =>
                {
                    if (options.SecretStore == null)
                    {
                        options.SecretStore = provider.GetService<ISecretReadOnlyStore>();
                    }
                })
                .AddTransient<IKeyValueStore, AzureBlobStorageClient, AzureBlobStorageClientOptions>(configurationSection: configurationSection, verifyOptions: (provider, options) =>
                {
                    if (options.SecretStore == null)
                    {
                        options.SecretStore = provider.GetService<ISecretReadOnlyStore>();
                    }
                });
        }

        public static IServiceCollection AddAzureBlobStorageKeyValueStore(this IServiceCollection services, Action<AzureBlobStorageClientOptionsBuilder> setup)
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNull(nameof(setup), setup);

            return services
                .AddTransient<IKeyValueReadOnlyStore, AzureBlobStorageClient, AzureBlobStorageClientOptions, AzureBlobStorageClientOptionsBuilder>(defaultSetup: setup, verifyOptions: (provider, options) =>
                {
                    if (options.SecretStore == null)
                    {
                        options.SecretStore = provider.GetService<ISecretReadOnlyStore>();
                    }
                })
                .AddTransient<IKeyValueStore, AzureBlobStorageClient, AzureBlobStorageClientOptions, AzureBlobStorageClientOptionsBuilder>(setup: setup, verifyOptions: (provider, options) =>
                {
                    if (options.SecretStore == null)
                    {
                        options.SecretStore = provider.GetService<ISecretReadOnlyStore>();
                    }
                });
        }

        public static IServiceCollection AddAzureBlobStoragePublicCertificateStore(this IServiceCollection services, string configurationSection = "AzureBlobStoragePublicCertificateStore")
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNullOrEmpty(nameof(configurationSection), configurationSection);

            return services
                .AddTransient<IPublicCertificateStore, AzureBlobStorageClient, AzureBlobStorageClientOptions, AzureBlobStorageClientOptionsBuilder>(
                    defaultSetup: b => b.WithServerTimeout(TimeSpan.FromSeconds(5)).WithMaximumExecutionTime(TimeSpan.FromSeconds(60)),
                    configurationSection: configurationSection, verifyOptions: (provider, options) =>
                    {
                        if (options.SecretStore == null)
                        {
                            options.SecretStore = provider.GetService<ISecretReadOnlyStore>();
                        }
                    });
        }

        public static IServiceCollection AddAzureBlobStoragePublicCertificateStore(this IServiceCollection services, Action<AzureBlobStorageClientOptionsBuilder> setup)
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNull(nameof(setup), setup);

            return services
                .AddTransient<IPublicCertificateStore, AzureBlobStorageClient, AzureBlobStorageClientOptions, AzureBlobStorageClientOptionsBuilder>(
                    defaultSetup: b => b.WithServerTimeout(TimeSpan.FromSeconds(5)).WithMaximumExecutionTime(TimeSpan.FromSeconds(60)),
                    setup: setup, verifyOptions: (provider, options) =>
                    {
                        if (options.SecretStore == null)
                        {
                            options.SecretStore = provider.GetService<ISecretReadOnlyStore>();
                        }
                    });
        }
    }
}

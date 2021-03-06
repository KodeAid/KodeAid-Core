// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using KodeAid;
using KodeAid.Azure.KeyVault;
using KodeAid.Security.Cryptography.X509Certificates;
using KodeAid.Security.Secrets;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureKeyVaultSecretStoreServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureKeyVaultSecretStore(this IServiceCollection services, string storeConfigurationKey = "AzureKeyVaultSecretStore")
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNullOrEmpty(nameof(storeConfigurationKey), storeConfigurationKey);

            return services.AddSingleton<ISecretReadOnlyStore>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>()?.GetSection(storeConfigurationKey)
                    ?? throw new InvalidOperationException($"Azure key vault store configuration section '{storeConfigurationKey}' not found.");
                var options = new AzureKeyVaultSecretStoreOptions();
                configuration.Bind(options);
                return new AzureKeyVaultSecretStore(options);
            });
        }

        public static IServiceCollection AddAzureKeyVaultSecretStore(this IServiceCollection services, Action<AzureKeyVaultSecretStoreOptionsBuilder> setupAction)
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNull(nameof(setupAction), setupAction);

            return services.AddSingleton<ISecretReadOnlyStore>(serviceProvider =>
            {
                var builder = new AzureKeyVaultSecretStoreOptionsBuilder();
                setupAction(builder);
                var options = builder.Build();
                return new AzureKeyVaultSecretStore(options);
            });
        }

        public static IServiceCollection AddAzureKeyVaultSecretStore(this IServiceCollection services, Action<IServiceProvider, AzureKeyVaultSecretStoreOptionsBuilder> setupAction)
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNull(nameof(setupAction), setupAction);

            return services.AddScoped<ISecretReadOnlyStore>(serviceProvider =>
            {
                var builder = new AzureKeyVaultSecretStoreOptionsBuilder();
                setupAction(serviceProvider, builder);
                var options = builder.Build();
                return new AzureKeyVaultSecretStore(options);
            });
        }

        public static IServiceCollection AddAzureKeyVaultPrivateCertificateStore(this IServiceCollection services, string storeConfigurationKey = "AzureKeyVaultPrivateCertificateStore")
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNullOrEmpty(nameof(storeConfigurationKey), storeConfigurationKey);

            return services.AddSingleton<IPrivateCertificateStore>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>()?.GetSection(storeConfigurationKey)
                    ?? throw new InvalidOperationException($"Azure key vault store configuration section '{storeConfigurationKey}' not found.");
                var options = new AzureKeyVaultSecretStoreOptions();
                configuration.Bind(options);
                return new AzureKeyVaultSecretStore(options);
            });
        }

        public static IServiceCollection AddAzureKeyVaultPrivateCertificateStore(this IServiceCollection services, Action<AzureKeyVaultSecretStoreOptionsBuilder> setupAction)
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNull(nameof(setupAction), setupAction);

            return services.AddSingleton<IPrivateCertificateStore>(serviceProvider =>
            {
                var builder = new AzureKeyVaultSecretStoreOptionsBuilder();
                setupAction(builder);
                var options = builder.Build();
                return new AzureKeyVaultSecretStore(options);
            });
        }

        public static IServiceCollection AddAzureKeyVaultPrivateCertificateStore(this IServiceCollection services, Action<IServiceProvider, AzureKeyVaultSecretStoreOptionsBuilder> setupAction)
        {
            ArgCheck.NotNull(nameof(services), services);
            ArgCheck.NotNull(nameof(setupAction), setupAction);

            return services.AddScoped<IPrivateCertificateStore>(serviceProvider =>
            {
                var builder = new AzureKeyVaultSecretStoreOptionsBuilder();
                setupAction(serviceProvider, builder);
                var options = builder.Build();
                return new AzureKeyVaultSecretStore(options);
            });
        }
    }
}

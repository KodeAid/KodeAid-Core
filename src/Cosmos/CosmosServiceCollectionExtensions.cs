// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using KodeAid;
using KodeAid.Azure.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CosmosServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, string connectionString, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, typeof(T).FullName, connectionString, options);
        }

        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, string connectionString, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, name, new CosmosClientRegistration() { ConnectionString = connectionString, Options = options });
        }

        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, typeof(T).FullName, accountEndpoint, authKeyOrResourceToken, options);
        }

        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, name, new CosmosClientRegistration() { AccountEndpoint = accountEndpoint, AuthKeyOrResourceToken = authKeyOrResourceToken, Options = options });
        }

        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, Func<IServiceProvider, CosmosClientRegistration> clientRegistrationFactory)
        {
            return AddCosmosClient(services, typeof(T).FullName, clientRegistrationFactory);
        }

        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, Func<IServiceProvider, CosmosClientRegistration> clientRegistrationFactory)
        {
            ArgCheck.NotNull(nameof(name), name);
            ArgCheck.NotNull(nameof(clientRegistrationFactory), clientRegistrationFactory);

            return AddCosmosClient(services, sp =>
            {
                var clientRegistration = clientRegistrationFactory(sp);
                clientRegistration.Name = name;
                return clientRegistration;
            });
        }

        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, CosmosClientRegistration clientRegistration)
        {
            return AddCosmosClient(services, typeof(T).FullName, clientRegistration);
        }

        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, CosmosClientRegistration clientRegistration)
        {
            ArgCheck.NotNull(nameof(name), name);
            ArgCheck.NotNull(nameof(clientRegistration), clientRegistration);

            clientRegistration.Name = name;
            return AddCosmosClient(services, clientRegistration);
        }

        private static IServiceCollection AddCosmosClient(this IServiceCollection services, Func<IServiceProvider, CosmosClientRegistration> clientRegistrationFactory)
        {
            services.TryAddSingleton<ICosmosClientFactory, DefaultCosmosClientFactory>();
            return services.AddSingleton(clientRegistrationFactory);
        }

        private static IServiceCollection AddCosmosClient(this IServiceCollection services, CosmosClientRegistration clientRegistration)
        {
            services.TryAddSingleton<ICosmosClientFactory, DefaultCosmosClientFactory>();
            return services.AddSingleton(clientRegistration);
        }
    }
}

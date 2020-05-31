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
        public static IServiceCollection AddCosmosClientFactory(this IServiceCollection services)
        {
            services.TryAddSingleton<ICosmosClientFactory, DefaultCosmosClientFactory>();
        }

        /// <summary>
        /// Adds a typed Cosmos client registration to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, string connectionString, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, typeof(T).FullName, connectionString, options);
        }

        /// <summary>
        /// Adds a named Cosmos client registration to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, string connectionString, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, name, new CosmosClientRegistration() { ConnectionString = connectionString, Options = options });
        }

        /// <summary>
        /// Adds a typed Cosmos client registration to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, typeof(T).FullName, accountEndpoint, authKeyOrResourceToken, options);
        }

        /// <summary>
        /// Adds a named Cosmos client registration to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, name, new CosmosClientRegistration() { AccountEndpoint = accountEndpoint, AuthKeyOrResourceToken = authKeyOrResourceToken, Options = options });
        }

        /// <summary>
        /// Adds a default Cosmos client registration by factory to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, Func<IServiceProvider, CosmosClientRegistration> clientRegistrationFactory)
        {
            return AddCosmosClient(services, CosmosClientRegistration.DefaultName, clientRegistrationFactory);
        }

        /// <summary>
        /// Adds a typed Cosmos client registration by factory to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, Func<IServiceProvider, CosmosClientRegistration> clientRegistrationFactory)
        {
            return AddCosmosClient(services, typeof(T).FullName, clientRegistrationFactory);
        }

        /// <summary>
        /// Adds a named Cosmos client registration by factory to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, Func<IServiceProvider, CosmosClientRegistration> clientRegistrationFactory)
        {
            ArgCheck.NotNull(nameof(name), name);
            ArgCheck.NotNull(nameof(clientRegistrationFactory), clientRegistrationFactory);

            return AddCosmosClientCore(services, sp =>
            {
                var clientRegistration = clientRegistrationFactory(sp);
                clientRegistration.Name = name;
                return clientRegistration;
            });
        }

        /// <summary>
        /// Adds a default Cosmos client registration to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, CosmosClientRegistration clientRegistration)
        {
            return AddCosmosClient(services, CosmosClientRegistration.DefaultName, clientRegistration);
        }

        /// <summary>
        /// Adds a typed Cosmos client registration to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, CosmosClientRegistration clientRegistration)
        {
            return AddCosmosClient(services, typeof(T).FullName, clientRegistration);
        }

        /// <summary>
        /// Adds a named Cosmos client registration to the service collection.
        /// </summary>
        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, CosmosClientRegistration clientRegistration)
        {
            ArgCheck.NotNull(nameof(name), name);
            ArgCheck.NotNull(nameof(clientRegistration), clientRegistration);

            clientRegistration.Name = name;
            return AddCosmosClientCore(services, clientRegistration);
        }

        private static IServiceCollection AddCosmosClientCore(this IServiceCollection services, Func<IServiceProvider, CosmosClientRegistration> clientRegistrationFactory)
        {
            services.TryAddSingleton<ICosmosClientRegistry, DefaultCosmosClientRegistry>();
            return services.AddSingleton(clientRegistrationFactory);
        }

        private static IServiceCollection AddCosmosClientCore(this IServiceCollection services, CosmosClientRegistration clientRegistration)
        {
            services.TryAddSingleton<ICosmosClientRegistry, DefaultCosmosClientRegistry>();
            return services.AddSingleton(clientRegistration);
        }
    }
}

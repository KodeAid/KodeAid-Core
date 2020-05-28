// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using KodeAid.Azure.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CosmosServiceCollectionExtensions
    {
        public static CosmosClient CreateClient<T>(this ICosmosClientFactory cosmosClientFactory)
        {
            return cosmosClientFactory.CreateClient(typeof(T).FullName);
        }

        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, string connectionString, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, typeof(T).FullName, connectionString, options);
        }

        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, string connectionString, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, new CosmosClientRegistration() { Name = name, ConnectionString = connectionString, Options = options });
        }

        public static IServiceCollection AddCosmosClient<T>(this IServiceCollection services, string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, typeof(T).FullName, accountEndpoint, authKeyOrResourceToken, options);
        }

        public static IServiceCollection AddCosmosClient(this IServiceCollection services, string name, string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options = null)
        {
            return AddCosmosClient(services, new CosmosClientRegistration() { Name = name, AccountEndpoint = accountEndpoint, AuthKeyOrResourceToken = authKeyOrResourceToken, Options = options });
        }

        private static IServiceCollection AddCosmosClient(this IServiceCollection services, CosmosClientRegistration clientRegistration)
        {
            services.TryAddSingleton<ICosmosClientFactory, DefaultCosmosClientFactory>();
            return services.AddSingleton(clientRegistration);
        }
    }
}

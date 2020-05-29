// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    public static class CosmosClientFactoryExtensions
    {
        /// <summary>
        /// Create default client.
        /// </summary>
        public static CosmosClient CreateClient(this ICosmosClientFactory cosmosClientFactory)
        {
            return cosmosClientFactory.CreateClient(CosmosClientRegistration.DefaultName);
        }

        /// <summary>
        /// Create client for type.
        /// </summary>
        public static CosmosClient CreateClient<T>(this ICosmosClientFactory cosmosClientFactory)
        {
            return cosmosClientFactory.CreateClient(typeof(T).FullName);
        }
    }
}

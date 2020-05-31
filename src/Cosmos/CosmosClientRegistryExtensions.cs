// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    public static class CosmosClientRegistryExtensions
    {
        /// <summary>
        /// Get default client.
        /// </summary>
        public static CosmosClient GetClient(this ICosmosClientRegistry cosmosClientRegistry)
        {
            return cosmosClientRegistry.GetClient(CosmosClientRegistration.DefaultName);
        }

        /// <summary>
        /// Get client for type.
        /// </summary>
        public static CosmosClient GetClient<T>(this ICosmosClientRegistry cosmosClientRegistry)
        {
            return cosmosClientRegistry.GetClient(typeof(T).FullName);
        }
    }
}

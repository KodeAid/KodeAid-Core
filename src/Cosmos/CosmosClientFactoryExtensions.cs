// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    public static class CosmosClientFactoryExtensions
    {
        public static CosmosClient CreateClient<T>(this ICosmosClientFactory cosmosClientFactory)
        {
            return cosmosClientFactory.CreateClient(typeof(T).FullName);
        }
    }
}

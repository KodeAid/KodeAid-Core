// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    public static class CosmosClientFactoryExtensions
    {
        public static CosmosClient CreateClientWithDefaultAzureCredential(this ICosmosClientFactory cosmosClientFactory, string accountEndpoint, CosmosClientOptions options)
            => cosmosClientFactory.CreateClient(accountEndpoint, new DefaultAzureCredential(), options);
    }
}

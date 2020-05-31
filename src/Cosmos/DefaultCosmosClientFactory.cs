// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    internal class DefaultCosmosClientFactory : ICosmosClientFactory
    {
        public CosmosClient CreateClient(string connectionString, CosmosClientOptions options) => new CosmosClient(connectionString, options);
        public CosmosClient CreateClient(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options) => new CosmosClient(accountEndpoint, authKeyOrResourceToken, options);
    }
}

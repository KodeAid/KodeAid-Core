// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Azure.Core;
using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    internal class DefaultCosmosClientFactory : ICosmosClientFactory
    {
        public CosmosClient CreateClient(string connectionString, CosmosClientOptions options) => new(connectionString, options);
        public CosmosClient CreateClient(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options) => new(accountEndpoint, authKeyOrResourceToken, options);
        public CosmosClient CreateClient(string accountEndpoint, TokenCredential tokenCredential, CosmosClientOptions options) => new(accountEndpoint, tokenCredential, options);
    }
}

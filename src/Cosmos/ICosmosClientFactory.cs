// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    public interface ICosmosClientFactory
    {
        CosmosClient CreateClient(string connectionString, CosmosClientOptions options);
        CosmosClient CreateClient(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions options);
    }
}

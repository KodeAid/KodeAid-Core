// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    public interface ICosmosClientRegistry
    {
        /// <summary>
        /// Get named client.
        /// </summary>
        CosmosClient GetClient(string name);
    }
}

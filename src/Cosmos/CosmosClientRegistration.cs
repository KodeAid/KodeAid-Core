// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    internal class CosmosClientRegistration
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string AccountEndpoint { get; set; }
        public string AuthKeyOrResourceToken { get; set; }
        public CosmosClientOptions Options { get; set; }
    }
}

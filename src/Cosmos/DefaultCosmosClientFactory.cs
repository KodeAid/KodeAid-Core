// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;

namespace KodeAid.Azure.Cosmos
{
    internal class DefaultCosmosClientFactory : ICosmosClientFactory
    {
        private readonly ConcurrentDictionary<string, CosmosClient> _clients = new ConcurrentDictionary<string, CosmosClient>();
        private readonly Dictionary<string, CosmosClientRegistration> _clientRegistrations;

        public DefaultCosmosClientFactory(IEnumerable<CosmosClientRegistration> clientRegistrations)
        {
            ArgCheck.NotNull(nameof(clientRegistrations), clientRegistrations);

            _clientRegistrations = clientRegistrations.ToDictionary(r => r.Name);
        }

        public CosmosClient CreateClient(string name)
        {
            return _clients.GetOrAdd(name, n =>
                _clientRegistrations.TryGetValue(n, out var r) ?
                r.ConnectionString != null ?
                    new CosmosClient(r.ConnectionString, r.Options) :
                    new CosmosClient(r.AccountEndpoint, r.AuthKeyOrResourceToken, r.Options) :
                throw new InvalidOperationException($"Cosmos client '{n}' is not registered."));
        }
    }
}

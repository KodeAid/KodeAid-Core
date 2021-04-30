// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Azure.Storage
{
    public sealed class AzureBlobStorageClientOptionsBuilder : IBuilder<AzureBlobStorageClientOptions>
    {
        private string _connectionString;
        private string _accountKey;
        private string _sasToken;
        private string _accountName;
        private string _endpointSuffix;
        private string _containerName;
        private string _defaultDirectoryRelativeAddress;
        private bool _useSnapshots;
        private bool _useManagedIdentity;
        private TimeSpan? _leaseDuration;
        private TimeSpan? _networkTimeout;

        public AzureBlobStorageClientOptions Build()
        {
            return new AzureBlobStorageClientOptions()
            {
                AccountName = _accountName,
                AccountKey = _accountKey,
                ConnectionString = _connectionString,
                ContainerName = _containerName,
                DefaultDirectoryRelativeAddress = _defaultDirectoryRelativeAddress,
                EndpointSuffix = _endpointSuffix,
                LeaseDuration = _leaseDuration,
                NetworkTimeout = _networkTimeout,
                SharedAccessSignature = _sasToken,
                UseManagedIdentity = _useManagedIdentity,
                UseSnapshots = _useSnapshots,
            };
        }

        public AzureBlobStorageClientOptionsBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithAccountKey(string accountKey, string accountName, string endpointSuffix = null)
        {
            _accountKey = accountKey;
            _accountName = accountName;
            _endpointSuffix = endpointSuffix;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithSharedAccessSignature(string sasToken, string accountName, string endpointSuffix = null)
        {
            _sasToken = sasToken;
            _accountName = accountName;
            _endpointSuffix = endpointSuffix;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithContainer(string containerName)
        {
            _containerName = containerName;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithDefaultDirectoryRelativeAddress(string directoryRelativeAddress)
        {
            _defaultDirectoryRelativeAddress = directoryRelativeAddress;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithManagedIdentity(bool managedIdentity)
        {
            _useManagedIdentity = managedIdentity;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithSnapshots(bool snapshots)
        {
            _useSnapshots = snapshots;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithLeaseDuration(TimeSpan duration)
        {
            _leaseDuration = duration;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithInfiniteLeaseDuration()
        {
            _leaseDuration = null;
            return this;
        }

        public AzureBlobStorageClientOptionsBuilder WithNetworkTimeout(TimeSpan? timeout)
        {
            _networkTimeout = timeout;
            return this;
        }
    }
}

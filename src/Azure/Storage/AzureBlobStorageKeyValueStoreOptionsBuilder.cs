// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace KodeAid.Azure.Storage
{
    public sealed class AzureBlobStorageKeyValueStoreOptionsBuilder
    {
        private CloudStorageAccount _storageAccount { get; set; }
        private string _containerName { get; set; }
        private string _defaultDirectoryRelativeAddress { get; set; }
        private TimeSpan? _leaseDuration { get; set; }

        public AzureBlobStorageKeyValueStoreOptions Build()
        {
            return new AzureBlobStorageKeyValueStoreOptions()
            {
                StorageAccount = _storageAccount,
                ContainerName = _containerName,
                DefaultDirectoryRelativeAddress = _defaultDirectoryRelativeAddress,
                LeaseDuration = _leaseDuration,
            }
            .Verify();
        }

        public AzureBlobStorageKeyValueStoreOptionsBuilder WithStorageAccount(CloudStorageAccount account)
        {
            ArgCheck.NotNull(nameof(account), account);

            _storageAccount = account;

            return this;
        }

        public AzureBlobStorageKeyValueStoreOptionsBuilder WithConnectionString(string connectionString)
        {
            ArgCheck.NotNullOrEmpty(nameof(connectionString), connectionString);

            _storageAccount = CloudStorageAccount.Parse(connectionString);

            return this;
        }

        public AzureBlobStorageKeyValueStoreOptionsBuilder WithSharedAccessSignature(string sasToken, string accountName, string endpointSuffix = null)
        {
            ArgCheck.NotNullOrEmpty(nameof(sasToken), sasToken);
            ArgCheck.NotNullOrEmpty(nameof(accountName), accountName);

            _storageAccount = new CloudStorageAccount(new StorageCredentials(sasToken), accountName, endpointSuffix ?? "core.windows.net", true);

            return this;
        }

        public AzureBlobStorageKeyValueStoreOptionsBuilder WithLeasesEnabled(TimeSpan duration)
        {
            // as per Azure storage lease duration requirements
            ArgCheck.GreaterThanOrEqualTo(nameof(duration), duration, TimeSpan.FromSeconds(15));
            ArgCheck.LessThanOrEqualTo(nameof(duration), duration, TimeSpan.FromSeconds(60));

            _leaseDuration = duration;

            return this;
        }

        public AzureBlobStorageKeyValueStoreOptionsBuilder WithLeasesDisabled()
        {
            _leaseDuration = null;

            return this;
        }
    }
}

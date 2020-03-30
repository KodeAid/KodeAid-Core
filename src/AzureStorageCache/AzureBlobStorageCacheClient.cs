// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using KodeAid.Serialization;
using KodeAid.Serialization.Binary;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace KodeAid.Caching.AzureStorage
{
    public class AzureBlobStorageCacheClient : CacheClientBase
    {
        private const string _cacheExpirationMetadataKey = "X-Cache-Expiration";
        private const string _dateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        private const string _defaultContainerName = "cache";
        private const string _defaultDefaultPartitionDirectory = "";
        private readonly string _containerName;
        private readonly string _defaultPartitionDirectory;
        private readonly IAsyncStreamSerializer _serializer;
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudBlobClient _client;
        private readonly CloudBlobContainer _container;
        private readonly BlobRequestOptions _options = new BlobRequestOptions() { RetryPolicy = new ExponentialRetry() }; //EncryptionPolicy = new TableEncryptionPolicy()

        public AzureBlobStorageCacheClient(string connectionString, ILogger<AzureBlobStorageCacheClient> logger, string containerName = _defaultContainerName, string defaultPartitionDirectory = _defaultDefaultPartitionDirectory, bool throwOnError = false)
            : this(connectionString, new DotNetBinarySerializer(), containerName, defaultPartitionDirectory, logger, throwOnError)
        {
        }

        public AzureBlobStorageCacheClient(string connectionString, IAsyncStreamSerializer serializer, string containerName, string defaultPartitionDirectory, ILogger logger, bool throwOnError)
            : base(throwOnError, logger)
        {
            ArgCheck.NotNullOrEmpty(nameof(connectionString), connectionString);
            ArgCheck.NotNull(nameof(serializer), serializer);
            ArgCheck.NotNullOrEmpty(nameof(containerName), containerName);
            _containerName = containerName;
            _defaultPartitionDirectory = defaultPartitionDirectory;
            _serializer = serializer;
            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _client = _storageAccount.CreateCloudBlobClient();
            _container = _client.GetContainerReference(_containerName);
        }

        protected override async Task<IEnumerable<CacheItem<T>>> GetItemsAsync<T>(IEnumerable<string> blobKeys, string partitionDirectory)
        {
            partitionDirectory = partitionDirectory ?? _defaultPartitionDirectory;
            ArgCheck.NotNull(nameof(partitionDirectory), partitionDirectory);

            var utcNow = DateTimeOffset.UtcNow;

            // cache hits
            var items = new List<CacheItem<T>>();

            foreach (var blobKey in blobKeys)
            {
                var blob = GetBlobReference(blobKey, partitionDirectory);

                var exists = await blob.ExistsAsync(_options, new OperationContext()).ConfigureAwait(false);
                if (!exists)
                {
                    continue;
                }

                // load metadata and properties
                await blob.FetchAttributesAsync(new AccessCondition(), _options, new OperationContext()).ConfigureAwait(false);

                // read expiration
                DateTimeOffset? expiration = null;
                if (blob.Metadata.TryGetValue(_cacheExpirationMetadataKey, out var expirationString) &&
                    DateTimeOffset.TryParseExact(expirationString, _dateTimeFormatString, null, DateTimeStyles.None, out var expirationDateTime))
                {
                    if (expirationDateTime <= utcNow)
                    {
                        // remove if expired
                        await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, new AccessCondition() { IfMatchETag = blob.Properties.ETag }, _options, new OperationContext()).ConfigureAwait(false);

                        // this one is expired so move on to the next one
                        continue;
                    }

                    // record the expiration for info later
                    expiration = expirationDateTime;
                }

                // cache hit...

                using (var stream = await blob.OpenReadAsync(new AccessCondition(), _options, new OperationContext()).ConfigureAwait(false))
                {
                    stream.Position = 0;
                    var item = new CacheItem<T>()
                    {
                        Key = blobKey,
                        Value = await _serializer.DeserializeFromStreamAsync<T>(stream).ConfigureAwait(false),
                        LastUpdated = blob.Properties.LastModified.GetValueOrDefault(),
                        Expiration = expiration,
                    };

                    items.Add(item);
                }
            }

            return items;
        }

        protected override async Task SetItemsAsync<T>(IEnumerable<CacheItem<T>> items, string partitionDirectory)
        {
            partitionDirectory = partitionDirectory ?? _defaultPartitionDirectory;
            ArgCheck.NotNull(nameof(partitionDirectory), partitionDirectory);

            var utcNow = DateTimeOffset.UtcNow;

            foreach (var item in items)
            {
                var blob = GetBlobReference(item.Key, partitionDirectory);
                using (var stream = await blob.OpenWriteAsync(new AccessCondition(), _options, new OperationContext()).ConfigureAwait(false))
                {
                    stream.Position = 0;
                    await _serializer.SerializeToStreamAsync(stream, item.Value).ConfigureAwait(false);
                }

                if (item.Expiration.HasValue)
                {
                    // load metadata and properties
                    await blob.FetchAttributesAsync(new AccessCondition(), _options, new OperationContext()).ConfigureAwait(false);

                    // set expiration
                    blob.Metadata[_cacheExpirationMetadataKey] = item.Expiration.Value.ToString(_dateTimeFormatString);
                    await blob.SetMetadataAsync(new AccessCondition(), _options, new OperationContext()).ConfigureAwait(false);
                }
            }
        }

        protected override async Task RemoveKeysAsync(IEnumerable<string> blobKeys, string partitionDirectory = null)
        {
            partitionDirectory = partitionDirectory ?? _defaultPartitionDirectory;
            ArgCheck.NotNull(nameof(partitionDirectory), partitionDirectory);

            foreach (var blobKey in blobKeys)
            {
                var blob = GetBlobReference(blobKey, partitionDirectory);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, new AccessCondition(), _options, new OperationContext()).ConfigureAwait(false);
            }
        }

        public Task CreateIfNotExistsAsync()
        {
            return _container.CreateIfNotExistsAsync();
        }

        public async Task RemoveExpiredAsync()
        {
            var utcNow = DateTimeOffset.UtcNow;

            BlobResultSegment segment = null;
            while (segment == null || segment.ContinuationToken != null)
            {
                segment = await _container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, null, segment?.ContinuationToken, _options, new OperationContext()).ConfigureAwait(false);
                foreach (ICloudBlob blob in segment.Results)
                {
                    // expired?
                    if (blob.Metadata.TryGetValue(_cacheExpirationMetadataKey, out var expirationString) &&
                        DateTimeOffset.TryParseExact(expirationString, _dateTimeFormatString, null, DateTimeStyles.None, out var expiration) &&
                        expiration <= utcNow)
                    {
                        await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, new AccessCondition() { IfMatchETag = blob.Properties.ETag }, _options, new OperationContext()).ConfigureAwait(false);
                    }
                }
            }
        }

        private CloudBlockBlob GetBlobReference(string blobKey, string partitionDirectory)
        {
            if (!string.IsNullOrEmpty(partitionDirectory))
            {
                return _container.GetDirectoryReference(partitionDirectory).GetBlockBlobReference(blobKey);
            }

            return _container.GetBlockBlobReference(blobKey);
        }
    }
}

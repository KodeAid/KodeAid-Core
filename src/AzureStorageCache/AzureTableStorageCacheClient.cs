// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KodeAid.Json;
using KodeAid.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace KodeAid.Caching.AzureStorage
{
    public class AzureTableStorageCacheClient : CacheClientBase
    {
        private const string _dateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        private const string _defaultTableName = "cache";
        private const string _defaultDefaultPartitionKey = "";
        private readonly string _tableName;
        private readonly string _defaultPartitionKey;
        private readonly ISerializer _serializer;
        private readonly bool _isBinarySerializer;
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _client;
        private readonly CloudTable _table;
        private readonly TableRequestOptions _options = new TableRequestOptions() { RetryPolicy = new ExponentialRetry() }; //EncryptionPolicy = new TableEncryptionPolicy()

        public AzureTableStorageCacheClient(string connectionString, ISerializer<string> serializer, ILogger<AzureTableStorageCacheClient> logger, string tableName = _defaultTableName, string defaultPartitionKey = _defaultDefaultPartitionKey, bool throwOnError = false)
            : this(connectionString, serializer, tableName, defaultPartitionKey, logger, throwOnError)
        {
            _isBinarySerializer = false;
        }

        public AzureTableStorageCacheClient(string connectionString, ISerializer<byte[]> serializer, ILogger<AzureTableStorageCacheClient> logger, string tableName = _defaultTableName, string defaultPartitionKey = _defaultDefaultPartitionKey, bool throwOnError = false)
            : this(connectionString, serializer, tableName, defaultPartitionKey, logger, throwOnError)
        {
            _isBinarySerializer = true;
        }

        public AzureTableStorageCacheClient(string connectionString, ILogger<AzureTableStorageCacheClient> logger, string tableName = _defaultTableName, string defaultPartitionKey = _defaultDefaultPartitionKey, bool throwOnError = false)
            : this(connectionString, new JsonSerializer(), tableName, defaultPartitionKey, logger, throwOnError)
        {
            _isBinarySerializer = false;
        }

        public AzureTableStorageCacheClient(CloudStorageAccount account, ISerializer<string> serializer, ILogger<AzureTableStorageCacheClient> logger, string tableName = _defaultTableName, string defaultPartitionKey = _defaultDefaultPartitionKey, bool throwOnError = false)
            : this(account, serializer, tableName, defaultPartitionKey, logger, throwOnError)
        {
            _isBinarySerializer = false;
        }

        public AzureTableStorageCacheClient(CloudStorageAccount account, ISerializer<byte[]> serializer, ILogger<AzureTableStorageCacheClient> logger, string tableName = _defaultTableName, string defaultPartitionKey = _defaultDefaultPartitionKey, bool throwOnError = false)
            : this(account, serializer, tableName, defaultPartitionKey, logger, throwOnError)
        {
            _isBinarySerializer = true;
        }

        public AzureTableStorageCacheClient(CloudStorageAccount account, ILogger<AzureTableStorageCacheClient> logger, string tableName = _defaultTableName, string defaultPartitionKey = _defaultDefaultPartitionKey, bool throwOnError = false)
            : this(account, new JsonSerializer(), tableName, defaultPartitionKey, logger, throwOnError)
        {
            _isBinarySerializer = false;
        }

        private AzureTableStorageCacheClient(string connectionString, ISerializer serializer, string tableName, string defaultPartitionKey, ILogger logger, bool throwOnError)
            : this(CloudStorageAccount.Parse(connectionString), new JsonSerializer(), tableName, defaultPartitionKey, logger, throwOnError)
        {

        }

        private AzureTableStorageCacheClient(CloudStorageAccount account, ISerializer serializer, string tableName, string defaultPartitionKey, ILogger logger, bool throwOnError)
            : base(throwOnError, logger)
        {
            ArgCheck.NotNull(nameof(account), account);
            ArgCheck.NotNull(nameof(serializer), serializer);
            ArgCheck.NotNullOrEmpty(nameof(tableName), tableName);
            _tableName = tableName;
            _defaultPartitionKey = defaultPartitionKey;
            _serializer = serializer;
            _storageAccount = account;
            _client = _storageAccount.CreateCloudTableClient();
            _table = _client.GetTableReference(_tableName);
        }

        protected override async Task<IEnumerable<CacheItem<T>>> GetItemsAsync<T>(IEnumerable<string> rowKeys, string partitionKey)
        {
            partitionKey = partitionKey ?? _defaultPartitionKey;
            ArgCheck.NotNull(nameof(partitionKey), partitionKey);

            var utcNow = DateTimeOffset.UtcNow;

            // cache hits
            var items = new List<CacheItem<T>>();

            foreach (var rowKey in rowKeys)
            {
                var retrieve = TableOperation.Retrieve<AzureTableStorageCacheEntry>(partitionKey, rowKey);
                var result = await ExecuteTableOperationAsync(retrieve).ConfigureAwait(false);

                // failed?
                if (result.HttpStatusCode / 100 != 2)
                {
                    if (result.HttpStatusCode == 404)
                    {
                        // cache miss
                        continue;
                    }

                    if (ThrowOnError)
                    {
                        throw new CacheAccessException($"Failed to read from Azure table storage cache for key '{rowKey}' in partition '{partitionKey}', HTTP status code returned was {result.HttpStatusCode}.");
                    }

                    continue;
                }

                // cache hit...

                var entry = (AzureTableStorageCacheEntry)result.Result;

                // expired?
                if (entry.Expiration.HasValue && entry.Expiration.Value <= utcNow)
                {
                    // remove if expired
                    var delete = TableOperation.Delete(new DynamicTableEntity(partitionKey, rowKey, result.Etag, null));
                    await ExecuteTableOperationAsync(delete).ConfigureAwait(false);

                    // this one is expired so move on to the next one
                    continue;
                }

                var item = new CacheItem<T>()
                {
                    Key = entry.RowKey,
                    Value = DeserializeValue<T>(entry.Value),
                    LastUpdated = entry.Timestamp,
                    Expiration = entry.Expiration,
                };

                items.Add(item);
            }

            return items;
        }

        protected override async Task SetItemsAsync<T>(IEnumerable<CacheItem<T>> items, string partitionKey)
        {
            partitionKey = partitionKey ?? _defaultPartitionKey;
            ArgCheck.NotNull(nameof(partitionKey), partitionKey);

            var utcNow = DateTimeOffset.UtcNow;

            foreach (var item in items)
            {
                var entry = new AzureTableStorageCacheEntry()
                {
                    PartitionKey = partitionKey,
                    RowKey = item.Key,
                    Timestamp = utcNow,
                    Expiration = item.Expiration,
                    Value = SerializeValue<T>(item.Value),
                };
                var insertOrReplace = TableOperation.InsertOrReplace(entry);
                var result = await ExecuteTableOperationAsync(insertOrReplace).ConfigureAwait(false);
                if (result.HttpStatusCode / 100 != 2 && ThrowOnError)
                {
                    throw new CacheAccessException($"Failed to write to Azure table storage cache for key '{entry.RowKey}' in partition '{partitionKey}', HTTP status code returned was {result.HttpStatusCode}.");
                }
            }
        }

        protected override async Task RemoveKeysAsync(IEnumerable<string> rowKeys, string partitionKey = null)
        {
            partitionKey = partitionKey ?? _defaultPartitionKey;
            ArgCheck.NotNull(nameof(partitionKey), partitionKey);

            var batch = new TableBatchOperation();
            foreach (var rowKey in rowKeys)
            {
                batch.Add(TableOperation.Delete(new DynamicTableEntity(partitionKey, rowKey) { ETag = "*" }));
                if (batch.Count == 100)
                {
                    await _table.ExecuteBatchAsync(batch).ConfigureAwait(false);
                    batch = new TableBatchOperation();
                }
            }
            if (batch.Count > 0)
            {
                await _table.ExecuteBatchAsync(batch).ConfigureAwait(false);
            }
        }

        public Task CreateIfNotExistsAsync()
        {
            return _table.CreateIfNotExistsAsync();
        }

        public async Task RemoveExpiredAsync()
        {
            var utcNow = DateTimeOffset.UtcNow;

            var filter = $"{nameof(AzureTableStorageCacheEntry.Expiration)} lt datetime'{utcNow.ToString(_dateTimeFormatString)}'";

            TableQuerySegment<DynamicTableEntity> segment = null;
            while (segment == null || segment.ContinuationToken != null)
            {
                var query = new TableQuery<DynamicTableEntity>()
                    .Where(filter)
                    .Take(100)
                    .Select(new List<string>() {
                        nameof(AzureTableStorageCacheEntry.PartitionKey),
                        nameof(AzureTableStorageCacheEntry.RowKey),
                        nameof(AzureTableStorageCacheEntry.ETag)});

                segment = await _table.ExecuteQuerySegmentedAsync(query, segment?.ContinuationToken, _options, new OperationContext()).ConfigureAwait(false);

                var batches = new Dictionary<string, TableBatchOperation>();

                foreach (var entity in segment.Results)
                {
                    if (!batches.TryGetValue(entity.PartitionKey, out var batch))
                    {
                        batches[entity.PartitionKey] = batch = new TableBatchOperation();
                    }

                    entity.ETag = "*";
                    batch.Add(TableOperation.Delete(entity));

                    if (batch.Count == 100)
                    {
                        await _table.ExecuteBatchAsync(batch).ConfigureAwait(false);
                        batches[entity.PartitionKey] = new TableBatchOperation();
                    }
                }

                foreach (var batch in batches.Values)
                {
                    if (batch.Count > 0)
                    {
                        await _table.ExecuteBatchAsync(batch).ConfigureAwait(false);
                    }
                }
            }
        }

        private Task<TableResult> ExecuteTableOperationAsync(TableOperation tableOperation)
        {
            return _table.ExecuteAsync(tableOperation, _options, new OperationContext());
        }

        private string SerializeValue<T>(T value)
        {
            if (value == null)
            {
                return null;
            }

            var data = _serializer.Serialize(value);
            if (_isBinarySerializer)
            {
                return ((byte[])data).ToBase64();
            }

            return (string)data;
        }

        private T DeserializeValue<T>(string value)
        {
            if (value == null)
            {
                return default;
            }

            object data = value;
            if (_isBinarySerializer)
            {
                data = value.FromBase64();
            }

            return _serializer.Deserialize<T>(data);
        }

        private sealed class AzureTableStorageCacheEntry : TableEntity
        {
            public DateTimeOffset? Expiration { get; set; }
            public string Value { get; set; }
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.Repositories;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KodeAid.Azure.Cosmos.Documents.Repositories
{
    public class DocumentRepository<TDocument> : ICrudRepositoryAsync<TDocument>
        where TDocument : class
    {
        private readonly IDocumentClient _client;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _documentTypeName;
        private readonly Uri _collectionUri;
        private readonly JsonSerializerSettings _serializerSettings;

        public DocumentRepository(
            IDocumentClient client,
            string databaseName, string collectionName, string documentTypeName = null,
            JsonSerializerSettings serializerSettings = null)
        {
            ArgCheck.NotNull(nameof(client), client);
            ArgCheck.NotNullOrEmpty(nameof(databaseName), databaseName);
            ArgCheck.NotNullOrEmpty(nameof(collectionName), collectionName);

            _client = client;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _documentTypeName = documentTypeName;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
            _serializerSettings = serializerSettings;
        }

        public DocumentRepository(
            string endpoint, string accessKeyOrResourceToken,
            string databaseName, string collectionName, string documentTypeName = null,
            JsonSerializerSettings serializerSettings = null,
            ConnectionPolicy connectionPolicy = null, ConsistencyLevel? consistencyLevel = null)
        {
            ArgCheck.NotNullOrEmpty(nameof(endpoint), endpoint);
            ArgCheck.NotNullOrEmpty(nameof(accessKeyOrResourceToken), accessKeyOrResourceToken);
            ArgCheck.NotNullOrEmpty(nameof(databaseName), databaseName);
            ArgCheck.NotNullOrEmpty(nameof(collectionName), collectionName);

            _databaseName = databaseName;
            _collectionName = collectionName;
            _documentTypeName = documentTypeName;
            _client = new DocumentClient(
              new Uri(endpoint), accessKeyOrResourceToken,
              serializerSettings, connectionPolicy, consistencyLevel);
            _serializerSettings = serializerSettings;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
        }

        public DocumentRepository(
            string endpoint, SecureString accessKey,
            string databaseName, string collectionName, string documentTypeName = null,
            JsonSerializerSettings serializerSettings = null,
            ConnectionPolicy connectionPolicy = null, ConsistencyLevel? consistencyLevel = null)
        {
            ArgCheck.NotNullOrEmpty(nameof(endpoint), endpoint);
            ArgCheck.NotNull(nameof(accessKey), accessKey);
            ArgCheck.NotNullOrEmpty(nameof(databaseName), databaseName);
            ArgCheck.NotNullOrEmpty(nameof(collectionName), collectionName);

            _databaseName = databaseName;
            _collectionName = collectionName;
            _documentTypeName = documentTypeName;
            _client = new DocumentClient(new Uri(endpoint), accessKey, serializerSettings, connectionPolicy, consistencyLevel);
            _serializerSettings = serializerSettings;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
        }

        public async Task<TDocument> GetAsync(string id, string partitionKey = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(id), id);
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var options = SetPartitionKey(partitionKey);
                var document = await _client.ReadDocumentAsync(GetDocumentUri(id), options).ConfigureAwait(false);
                return (TDocument)(dynamic)document.Resource;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }
                throw;
            }
        }

        public Task<IEnumerable<TDocument>> GetAllAsync(object partitionKey = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return FindAsync(null, partitionKey);
        }

        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> predicate, object partitionKey = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var options = new FeedOptions { MaxItemCount = -1 };
            options = SetPartitionKeyForFeed(partitionKey, options);
            var queryBuilder = _documentTypeName != null ?
                _client.CreateTypedDocumentQuery<TDocument>(_collectionUri, _documentTypeName, options) :
                _client.CreateDocumentQuery<TDocument>(_collectionUri, options);

            if (predicate != null)
            {
                queryBuilder = queryBuilder.Where(predicate);
            }

            var query = queryBuilder.AsDocumentQuery();

            var results = new List<TDocument>();
            while (query.HasMoreResults)
            {
                var documents = await query.ExecuteNextAsync<Document>().ConfigureAwait(false);
                results.AddRange(documents.Select(d => (TDocument)(dynamic)d));
            }
            return results;
        }

        public async Task<string> AddAsync(TDocument document, object partitionKey = null, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(document), document);
            cancellationToken.ThrowIfCancellationRequested();

            var options = SetPartitionKey(partitionKey);
            options = SetTimeToLive(ttl, options);
            if (_documentTypeName != null)
            {
                return (await _client.CreateTypedDocumentAsync(_collectionUri, document, _documentTypeName, _serializerSettings, options).ConfigureAwait(false)).Resource.Id;
            }
            return (await _client.CreateDocumentAsync(_collectionUri, document, options).ConfigureAwait(false)).Resource.Id;
        }

        public async Task<IEnumerable<string>> AddRangeAsync(IEnumerable<TDocument> documents, object partitionKey = null, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(documents), documents);
            cancellationToken.ThrowIfCancellationRequested();

            return (await Task.WhenAll(documents.Select(document => AddAsync(document, partitionKey, ttl)).ToList()).ConfigureAwait(false)).ToList();
        }

        public Task UpdateAsync(TDocument document, object partitionKey = null, string eTag = null, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(document), document);
            cancellationToken.ThrowIfCancellationRequested();

            var options = SetPartitionKey(partitionKey);
            options = SetETag(eTag, options);
            options = SetTimeToLive(ttl, options);
            if (_documentTypeName != null)
            {
                return _client.ReplaceTypedDocumentAsync(GetDocumentUri(document), document, _documentTypeName, _serializerSettings, options);
            }
            return _client.ReplaceDocumentAsync(GetDocumentUri(document), document, options);
        }

        public Task UpdateRangeAsync(IEnumerable<TDocument> documents, object partitionKey = null, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(documents), documents);
            cancellationToken.ThrowIfCancellationRequested();

            return Task.WhenAll(documents.Select(document => UpdateAsync(document, partitionKey, null, ttl)).ToList());
        }

        public async Task<string> SaveAsync(TDocument document, string documentType = null, object partitionKey = null, string eTag = null, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(document), document);
            cancellationToken.ThrowIfCancellationRequested();

            var options = SetPartitionKey(partitionKey);
            options = SetETag(eTag, options);
            options = SetTimeToLive(ttl, options);
            if (_documentTypeName != null)
            {
                return (await _client.UpsertTypedDocumentAsync(_collectionUri, document, _documentTypeName, _serializerSettings, options).ConfigureAwait(false)).Resource.Id;
            }
            return (await _client.UpsertDocumentAsync(_collectionUri, document, options).ConfigureAwait(false)).Resource.Id;
        }

        public async Task<IEnumerable<string>> SaveRangeAsync(IEnumerable<TDocument> documents, string documentType = null, object partitionKey = null, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(documents), documents);
            cancellationToken.ThrowIfCancellationRequested();

            return (await Task.WhenAll(documents.Select(document => SaveAsync(document, documentType, partitionKey, null, ttl)).ToList()).ConfigureAwait(false)).ToList();
        }

        public Task RemoveAsync(TDocument document, object partitionKey = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(document), document);
            cancellationToken.ThrowIfCancellationRequested();

            var options = SetPartitionKey(partitionKey);
            return _client.DeleteDocumentAsync(GetDocumentUri(GetIdFromDocument(document)), options);
        }

        public Task RemoveRangeAsync(IEnumerable<TDocument> documents, object partitionKey = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(documents), documents);
            cancellationToken.ThrowIfCancellationRequested();

            return Task.WhenAll(documents.Select(document => RemoveAsync(document, partitionKey)).ToList());
        }

        public Task RemoveAsync(string id, object partitionKey = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(id), id);
            cancellationToken.ThrowIfCancellationRequested();

            var options = SetPartitionKey(partitionKey);
            return _client.DeleteDocumentAsync(GetDocumentUri(id), options);
        }

        public Task RemoveRangeAsync(IEnumerable<string> ids, object partitionKey = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(ids), ids);
            cancellationToken.ThrowIfCancellationRequested();

            return Task.WhenAll(ids.Select(id => RemoveAsync(id, partitionKey)).ToList());
        }

        public async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var documents = await GetAllAsync().ConfigureAwait(false);
            await RemoveRangeAsync(documents).ConfigureAwait(false);
        }

        public Task ClearAsync(object partitionKey, CancellationToken cancellationToken = default)
        {
            if (partitionKey == null)
                return ClearAsync(cancellationToken);
            return ClearAsync(new[] { partitionKey }, cancellationToken);
        }

        public async Task ClearAsync(IEnumerable<object> partitionKeys, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNull(nameof(partitionKeys), partitionKeys);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var partitionKey in partitionKeys)
            {
                var documents = await GetAllAsync(partitionKey).ConfigureAwait(false);
                await RemoveRangeAsync(documents, partitionKey).ConfigureAwait(false);
            }
        }

        protected virtual string GetIdFromDocument(TDocument document)
        {
            return
                (document as IIdentifiable<string>)?.Id ??
                (_serializerSettings != null ?
                    (string)JObject.Parse(JsonConvert.SerializeObject(document, _serializerSettings))["id"] :
                    (string)JObject.FromObject(document)["id"]) ??
                throw new ArgumentException("No ID found on document.", nameof(document));
        }

        Task<TDocument> IReadRepositoryAsync<TDocument>.GetAsync(object id, CancellationToken cancellationToken)
        {
            return GetAsync(id?.ToString(), null);
        }

        Task<IEnumerable<TDocument>> IReadRepositoryAsync<TDocument>.GetAllAsync(CancellationToken cancellationToken)
        {
            return GetAllAsync(null);
        }

        Task<IEnumerable<TDocument>> IReadRepositoryAsync<TDocument>.FindAsync(Expression<Func<TDocument, bool>> predicate, CancellationToken cancellationToken)
        {
            return FindAsync(predicate, null);
        }

        Task ICrudRepositoryAsync<TDocument>.AddAsync(TDocument document, CancellationToken cancellationToken)
        {
            return AddAsync(document, null);
        }

        Task ICrudRepositoryAsync<TDocument>.AddRangeAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken)
        {
            return AddRangeAsync(documents, null);
        }

        Task ICrudRepositoryAsync<TDocument>.RemoveAsync(TDocument document, CancellationToken cancellationToken)
        {
            return RemoveAsync(document, null);
        }

        Task ICrudRepositoryAsync<TDocument>.RemoveRangeAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken)
        {
            return RemoveRangeAsync(documents, null);
        }

        Task ICrudRepositoryAsync<TDocument>.UpdateAsync(TDocument document, CancellationToken cancellationToken)
        {
            return UpdateAsync(document, null);
        }

        Task ICrudRepositoryAsync<TDocument>.UpdateRangeAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken)
        {
            return UpdateRangeAsync(documents, null);
        }

        private Uri GetDocumentUri(string id)
        {
            return UriFactory.CreateDocumentUri(_databaseName, _collectionName, id);
        }

        private Uri GetDocumentUri(TDocument document)
        {
            return GetDocumentUri(GetIdFromDocument(document));
        }

        private FeedOptions SetPartitionKeyForFeed(object partitionKey, FeedOptions options = null)
        {
            if (partitionKey == null)
                return options;
            if (options == null)
                options = new FeedOptions();
            options.PartitionKey = new PartitionKey(partitionKey);
            return options;
        }

        private RequestOptions SetPartitionKey(object partitionKey, RequestOptions options = null)
        {
            if (partitionKey == null)
                return options;
            if (options == null)
                options = new RequestOptions();
            options.PartitionKey = new PartitionKey(partitionKey);
            return options;
        }

        private RequestOptions SetETag(string eTag, RequestOptions options = null)
        {
            if (eTag == null)
                return options;
            if (options == null)
                options = new RequestOptions();
            options.AccessCondition = new AccessCondition() { Type = AccessConditionType.IfMatch, Condition = eTag };
            return options;
        }

        private RequestOptions SetTimeToLive(TimeSpan? ttl, RequestOptions options = null)
        {
            if (!ttl.HasValue)
                return options;
            if (options == null)
                options = new RequestOptions();
            options.ResourceTokenExpirySeconds = (int)ttl.Value.TotalSeconds;
            return options;
        }
    }
}

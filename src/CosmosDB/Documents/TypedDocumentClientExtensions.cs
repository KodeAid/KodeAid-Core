// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KodeAid.Azure.Cosmos.Documents;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Documents.Client
{
    public static class TypedDocumentClientExtensions
    {
        public static Func<Type, string> DocumentTypeResolver { get; set; } = (type) => type?.GetCustomAttributes(typeof(DocumentTypeAttribute), true).OfType<DocumentTypeAttribute>()?.FirstOrDefault()?.Type ?? type?.FullName;
        public static JsonSerializerSettings SerializerSettings { get; set; }

        public static IQueryable<Document> CreateTypedDocumentQuery(this IDocumentClient client, string collectionLink, string documentType, FeedOptions options = null)
        {
            return client.CreateDocumentQuery(collectionLink, options).Where(d => ((DocumentType)(object)d).Type == documentType).AsQueryable();
        }

        public static IQueryable<Document> CreateTypedDocumentQuery(this IDocumentClient client, Uri collectionUri, string documentType, FeedOptions options = null)
        {
            return client.CreateDocumentQuery(collectionUri, options).Where(d => ((DocumentType)(object)d).Type == documentType).AsQueryable();
        }

        public static IQueryable<T> CreateTypedDocumentQuery<T>(this IDocumentClient client, string collectionLink, string documentType = null, FeedOptions options = null)
        {
            documentType = GetDocumentType<T>(documentType);
            return client.CreateDocumentQuery<T>(collectionLink, options).Where(d => ((DocumentType)(object)d).Type == documentType).AsQueryable();
        }

        public static IQueryable<T> CreateTypedDocumentQuery<T>(this IDocumentClient client, Uri collectionUri, string documentType = null, FeedOptions options = null)
        {
            documentType = GetDocumentType<T>(documentType);
            return client.CreateDocumentQuery<T>(collectionUri, options).Where(d => ((DocumentType)(object)d).Type == documentType).AsQueryable();
        }

        public static Task<ResourceResponse<Document>> CreateTypedDocumentAsync(this IDocumentClient client, string collectionLink, object document, string documentType = null, JsonSerializerSettings serializerSettings = null, RequestOptions options = null, bool disableAutomaticIdGeneration = false)
        {
            document = GetDocumentObject(document, documentType, serializerSettings);
            return client.CreateDocumentAsync(collectionLink, document, options, disableAutomaticIdGeneration);
        }

        public static Task<ResourceResponse<Document>> CreateTypedDocumentAsync(this IDocumentClient client, Uri collectionUri, object document, string documentType = null, JsonSerializerSettings serializerSettings = null, RequestOptions options = null, bool disableAutomaticIdGeneration = false)
        {
            document = GetDocumentObject(document, documentType, serializerSettings);
            return client.CreateDocumentAsync(collectionUri, document, options, disableAutomaticIdGeneration);
        }

        public static Task<int> DeleteAllTypedDocumentsAsync(this IDocumentClient client, string collectionLink, string documentType, FeedOptions options = null)
        {
            var query = client.CreateTypedDocumentQuery(collectionLink, documentType, options).AsDocumentQuery();
            return DeleteDocumentsInQueryAsync(client, query);
        }

        public static Task<int> DeleteAllTypedDocumentsAsync(this IDocumentClient client, Uri collectionUri, string documentType, FeedOptions options = null)
        {
            var query = client.CreateTypedDocumentQuery(collectionUri, documentType, options).AsDocumentQuery();
            return DeleteDocumentsInQueryAsync(client, query);
        }

        public static Task<int> DeleteAllTypedDocumentsAsync<T>(this IDocumentClient client, string collectionLink, FeedOptions options = null)
        {
            var query = client.CreateTypedDocumentQuery<T>(collectionLink, options: options).AsDocumentQuery();
            return DeleteDocumentsInQueryAsync(client, query);
        }

        public static Task<int> DeleteAllTypedDocumentsAsync<T>(this IDocumentClient client, Uri collectionUri, FeedOptions options = null)
        {
            var query = client.CreateTypedDocumentQuery<T>(collectionUri, options: options).AsDocumentQuery();
            return DeleteDocumentsInQueryAsync(client, query);
        }

        public static Task<ResourceResponse<Document>> ReplaceTypedDocumentAsync(this IDocumentClient client, string documentLink, object document, string documentType = null, JsonSerializerSettings serializerSettings = null, RequestOptions options = null)
        {
            document = GetDocumentObject(document, documentType, serializerSettings);
            return client.ReplaceDocumentAsync(documentLink, document, options);
        }

        public static Task<ResourceResponse<Document>> ReplaceTypedDocumentAsync(this IDocumentClient client, Uri documentUri, object document, string documentType = null, JsonSerializerSettings serializerSettings = null, RequestOptions options = null)
        {
            document = GetDocumentObject(document, documentType, serializerSettings);
            return client.ReplaceDocumentAsync(documentUri, document, options);
        }

        public static Task<ResourceResponse<Document>> UpsertTypedDocumentAsync(this IDocumentClient client, string collectionLink, object document, string documentType = null, JsonSerializerSettings serializerSettings = null, RequestOptions options = null, bool disableAutomaticIdGeneration = false)
        {
            document = GetDocumentObject(document, documentType, serializerSettings);
            return client.UpsertDocumentAsync(collectionLink, document, options, disableAutomaticIdGeneration);
        }

        public static Task<ResourceResponse<Document>> UpsertTypedDocumentAsync(this IDocumentClient client, Uri collectionUri, object document, string documentType = null, JsonSerializerSettings serializerSettings = null, RequestOptions options = null, bool disableAutomaticIdGeneration = false)
        {
            document = GetDocumentObject(document, documentType, serializerSettings);
            return client.UpsertDocumentAsync(collectionUri, document, options, disableAutomaticIdGeneration);
        }

        private static string GetDocumentType<T>(string documentType)
        {
            return documentType ?? DocumentTypeResolver(typeof(T)) ?? throw new InvalidOperationException("Could not resolve document type.");
        }

        private static object GetDocumentObject(object document, string documentType, JsonSerializerSettings serializerSettings)
        {
            documentType = documentType ?? DocumentTypeResolver(document?.GetType()) ?? throw new InvalidOperationException("Could not resolve document type.");
            if (documentType == null)
                return document;
            var jObject = serializerSettings != null || SerializerSettings != null ?
              JObject.Parse(JsonConvert.SerializeObject(document, serializerSettings ?? SerializerSettings)) :
              JObject.FromObject(document);
            jObject[DocumentType.DocumentTypeJsonPropertyName] = documentType;
            return jObject;
        }

        private static async Task<int> DeleteDocumentsInQueryAsync<T>(IDocumentClient client, IDocumentQuery<T> query)
        {
            var linksToDelete = new List<string>();
            while (query.HasMoreResults)
            {
                var documents = await query.ExecuteNextAsync<Document>().ConfigureAwait(false);
                linksToDelete.AddRange(documents.Select(d => d.SelfLink));
            }
            foreach (var link in linksToDelete)
                await client.DeleteDocumentAsync(link).ConfigureAwait(false);
            return linksToDelete.Count;
        }
    }
}

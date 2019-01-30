// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.Security.Cryptography.X509Certificates;
using KodeAid.Security.Secrets;
using KodeAid.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace KodeAid.Azure.Storage
{
    public class AzureBlobStorageKeyValueStore : IKeyValueStore, IPublicCertificateStore, IInitializableAsync
    {
        private const string _expiresMetadataKey = "Expires";
        private const string _dateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        private readonly string _defaultDirectoryRelativeAddress;
        private readonly ISecretReadOnlyStore _secretStore;
        private readonly string _connectionStringSecretName;
        private readonly string _sharedAccessSignatureSecretName;
        private readonly string _containerName;
        private readonly string _accountName;
        private readonly string _endpointSuffix;
        private CloudBlobContainer _container;
        private readonly TimeSpan? _leaseDuration;
        private readonly BlobRequestOptions _requestOptions = new BlobRequestOptions() { RetryPolicy = new ExponentialRetry() }; //EncryptionPolicy = new TableEncryptionPolicy()
        private readonly bool _deleteExpiredDuringRequests = false;

        public AzureBlobStorageKeyValueStore(AzureBlobStorageKeyValueStoreOptions options)
            : this(options, null)
        {
        }

        public AzureBlobStorageKeyValueStore(AzureBlobStorageKeyValueStoreOptions options, ISecretReadOnlyStore secretStore)
        {
            ArgCheck.NotNull(nameof(options), options);

            if (options.SecretStore == null)
            {
                options.SecretStore = secretStore;
            }

            options.Verify();

            if (options.StorageAccount != null)
            {
                _container = options.StorageAccount.CreateCloudBlobClient().GetContainerReference(options.ContainerName);
            }
            else
            {
                _secretStore = options.SecretStore;
                _connectionStringSecretName = options.ConnectionStringSecretName;
                _sharedAccessSignatureSecretName = options.SharedAccessSignatureSecretName;
                _accountName = options.AccountName;
                _endpointSuffix = options.EndpointSuffix;
                _containerName = options.ContainerName;
            }

            _defaultDirectoryRelativeAddress = options.DefaultDirectoryRelativeAddress;
            _leaseDuration = options.LeaseDuration;
        }

        public bool IsInitialized => _container != null;

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
            {
                return;
            }

            var secret = await _secretStore.GetSecretAsync(_connectionStringSecretName ?? _sharedAccessSignatureSecretName, cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(_connectionStringSecretName))
            {
                _container = CloudStorageAccount.Parse(secret.Unsecure()).CreateCloudBlobClient().GetContainerReference(_containerName);
            }
            else
            {
                _container = new CloudStorageAccount(new StorageCredentials(secret.Unsecure()), _accountName, _endpointSuffix ?? "core.windows.net", true).CreateCloudBlobClient().GetContainerReference(_containerName);
            }
        }

        public async Task<BlobStringResult> GetStringAsync(string blobName, string directoryRelativeAddress = null, string ifNoneMatchETag = null, DateTimeOffset? ifModifiedSinceTime = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default)
        {
            using (var result = await GetStreamAsync(blobName, directoryRelativeAddress, ifNoneMatchETag, ifModifiedSinceTime, throwOnNotFound, cancellationToken).ConfigureAwait(false))
            {
                if (result.Status != GetBlobStatus.OK)
                {
                    return new BlobStringResult(result);
                }

                using (result.Contents)
                using (var ms = new MemoryStream())
                {
                    await result.Contents.CopyToAsync(ms, 81920, cancellationToken).ConfigureAwait(false);
                    await ms.FlushAsync(cancellationToken).ConfigureAwait(false);

                    var encoding = Encoding.GetEncoding(result.ContentEncoding) ?? Encoding.UTF8;

                    return new BlobStringResult(result, encoding.GetString(ms.ToArray()));
                }
            }
        }

        public async Task<BlobBytesResult> GetBytesAsync(string blobName, string directoryRelativeAddress = null, string ifNoneMatchETag = null, DateTimeOffset? ifModifiedSinceTime = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default)
        {
            using (var result = await GetStreamAsync(blobName, directoryRelativeAddress, ifNoneMatchETag, ifModifiedSinceTime, throwOnNotFound, cancellationToken).ConfigureAwait(false))
            {
                if (result.Status != GetBlobStatus.OK)
                {
                    return new BlobBytesResult(result);
                }

                using (result.Contents)
                using (var ms = new MemoryStream())
                {
                    await result.Contents.CopyToAsync(ms, 81920, cancellationToken).ConfigureAwait(false);
                    await ms.FlushAsync(cancellationToken).ConfigureAwait(false);

                    return new BlobBytesResult(result, ms.ToArray());
                }
            }
        }

        public async Task<BlobStreamResult> GetStreamAsync(string blobName, string directoryRelativeAddress = null, string ifNoneMatchETag = null, DateTimeOffset? ifModifiedSinceTime = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            directoryRelativeAddress = directoryRelativeAddress ?? _defaultDirectoryRelativeAddress;
            ArgCheck.NotNull(nameof(directoryRelativeAddress), directoryRelativeAddress);

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

            //if (!await blob.ExistsAsync(_options, new OperationContext()).ConfigureAwait(false))
            //{
            //    return new BlobResult<string>()
            //    {
            //        BlobUri = blobUri,
            //        DirectoryUri = directoryUri,
            //        Status = StoreResultStatus.NotFound
            //    };
            //}


            try
            {

                // load metadata and properties
                await blob.FetchAttributesAsync(new AccessCondition(), _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                // 404: not found
                if (!throwOnNotFound && ex.RequestInformation.HttpStatusCode == 404)
                {
                    return new BlobStreamResult()
                    {
                        BlobName = blobName,
                        DirectoryRelativeAddress = directoryRelativeAddress,
                        Status = GetBlobStatus.NotFound,
                    };
                }

                throw;
            }

            // read expiration
            DateTimeOffset? expires = null;
            if (blob.Metadata.TryGetValue(_expiresMetadataKey, out var expiresString) &&
                DateTimeOffset.TryParseExact(expiresString, _dateTimeFormatString, null, DateTimeStyles.None, out var expiresDateTime))
            {
                if (expiresDateTime <= DateTimeOffset.UtcNow)
                {
                    if (_deleteExpiredDuringRequests)
                    {
                        try
                        {
                            // remove if expired
                            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, new AccessCondition() { IfMatchETag = blob.Properties.ETag }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                        }
                        catch (StorageException)
                        {
                        }
                    }

                    // this one is expired, effectively not found
                    return new BlobStreamResult()
                    {
                        BlobName = blobName,
                        DirectoryRelativeAddress = directoryRelativeAddress,
                        Status = GetBlobStatus.NotFound,
                    };
                }

                // record the expiration for info later
                expires = expiresDateTime;
            }

            if (ifNoneMatchETag != null && blob.Properties.ETag != null && blob.Properties.ETag == ifNoneMatchETag)
            {
                return new BlobStreamResult()
                {
                    BlobName = blobName,
                    DirectoryRelativeAddress = directoryRelativeAddress,
                    Status = GetBlobStatus.NotModified,
                    ContentType = blob.Properties.ContentType,
                    ContentEncoding = blob.Properties.ContentEncoding,
                    ETag = blob.Properties.ETag,
                    Created = blob.Properties.Created,
                    LastModified = blob.Properties.LastModified,
                    Expires = expires,
                };
            }

            if (ifModifiedSinceTime.HasValue && blob.Properties.LastModified.HasValue && blob.Properties.LastModified <= ifModifiedSinceTime)
            {
                return new BlobStreamResult()
                {
                    BlobName = blobName,
                    DirectoryRelativeAddress = directoryRelativeAddress,
                    Status = GetBlobStatus.NotModified,
                    ContentType = blob.Properties.ContentType,
                    ContentEncoding = blob.Properties.ContentEncoding,
                    ETag = blob.Properties.ETag,
                    Created = blob.Properties.Created,
                    LastModified = blob.Properties.LastModified,
                    Expires = expires,
                };
            }

            try
            {
                var stream = await blob.OpenReadAsync(new AccessCondition() { IfNoneMatchETag = ifNoneMatchETag, IfModifiedSinceTime = ifModifiedSinceTime }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                return new BlobStreamResult(stream)
                {
                    BlobName = blobName,
                    DirectoryRelativeAddress = directoryRelativeAddress,
                    Status = GetBlobStatus.OK,
                    ContentType = blob.Properties.ContentType,
                    ContentEncoding = blob.Properties.ContentEncoding,
                    ETag = blob.Properties.ETag,
                    Created = blob.Properties.Created,
                    LastModified = blob.Properties.LastModified,
                    Expires = expires,
                };
            }
            catch (StorageException ex)
            {
                // 404: not found
                if (!throwOnNotFound && ex.RequestInformation.HttpStatusCode == 404)
                {
                    return new BlobStreamResult()
                    {
                        BlobName = blobName,
                        DirectoryRelativeAddress = directoryRelativeAddress,
                        Status = GetBlobStatus.NotFound,
                    };
                }

                // 304: not modified; 412: precondition failed
                // use cached version
                if (ex.RequestInformation.HttpStatusCode == 304 || ex.RequestInformation.HttpStatusCode == 412)
                {
                    return new BlobStreamResult()
                    {
                        BlobName = blobName,
                        DirectoryRelativeAddress = directoryRelativeAddress,
                        Status = GetBlobStatus.NotModified,
                        ContentType = blob.Properties.ContentType,
                        ContentEncoding = blob.Properties.ContentEncoding,
                    };
                }

                throw;
            }
        }

        public Task<BlobResult> PutAsync(string blobName, string contents, string directoryRelativeAddress = null, string contentType = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            return PutAsync(blobName, contents, Encoding.UTF8, directoryRelativeAddress, contentType, ifMatchETag, ifNotModifiedSinceTime, absoluteExpiration, cancellationToken);
        }

        public Task<BlobResult> PutAsync(string blobName, string contents, Encoding encoding, string directoryRelativeAddress = null, string contentType = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            ArgCheck.NotNull(nameof(contents), contents);

            encoding = encoding ?? Encoding.UTF8;

            return PutAsync(blobName, encoding.GetBytes(contents), directoryRelativeAddress, contentType, encoding.WebName, ifMatchETag, ifNotModifiedSinceTime, absoluteExpiration, cancellationToken);
        }

        public Task<BlobResult> PutAsync(string blobName, byte[] contents, string directoryRelativeAddress = null, string contentType = null, string contentEncoding = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            ArgCheck.NotNull(nameof(contents), contents);

            using (var ms = new MemoryStream(contents, false))
            {
                ms.Position = 0;
                return PutAsync(blobName, ms, directoryRelativeAddress, contentType, contentEncoding, ifMatchETag, ifNotModifiedSinceTime, absoluteExpiration, cancellationToken);
            }
        }

        public async Task<BlobResult> PutAsync(string blobName, Stream contents, string directoryRelativeAddress = null, string contentType = null, string contentEncoding = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            ArgCheck.NotNull(nameof(contents), contents);
            directoryRelativeAddress = directoryRelativeAddress ?? _defaultDirectoryRelativeAddress;
            ArgCheck.NotNull(nameof(directoryRelativeAddress), directoryRelativeAddress);

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

            var leaseId = (string)null;

            try
            {
                try
                {
                    if (_leaseDuration.HasValue)
                    {
                        leaseId = await blob.AcquireLeaseAsync(TimeSpan.FromSeconds(60), null, new AccessCondition(), _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (StorageException ex)
                {
                    // 404: not found
                    if (ex.RequestInformation.HttpStatusCode == 404)
                    {
                        leaseId = null;
                    }

                    throw;
                }

                // !! once we open the blob stream it will have been created, so we should no longer honour the cancellation token otherwise we leave the blob in a bad state

                using (var stream = await blob.OpenWriteAsync(new AccessCondition() { IfMatchETag = ifMatchETag, IfNotModifiedSinceTime = ifNotModifiedSinceTime }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false))
                {
                    stream.Position = 0;
                    await contents.CopyToAsync(stream).ConfigureAwait(false);
                    //await stream.WriteAsync(contents, 0, contents.Length).ConfigureAwait(false);
                    await stream.FlushAsync().ConfigureAwait(false);
                }

                // load metadata and properties
                await blob.FetchAttributesAsync(new AccessCondition(), _requestOptions, new OperationContext()).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(contentType) || !string.IsNullOrWhiteSpace(contentEncoding))
                {
                    if (!string.IsNullOrWhiteSpace(contentType))
                    {
                        blob.Properties.ContentType = contentType;
                    }

                    if (!string.IsNullOrWhiteSpace(contentEncoding))
                    {
                        blob.Properties.ContentEncoding = contentEncoding;
                    }

                    await blob.SetPropertiesAsync(new AccessCondition(), _requestOptions, new OperationContext()).ConfigureAwait(false);
                }

                if (absoluteExpiration.HasValue)
                {
                    // set expiration
                    blob.Metadata[_expiresMetadataKey] = absoluteExpiration.Value.ToString(_dateTimeFormatString);
                    await blob.SetMetadataAsync(new AccessCondition(), _requestOptions, new OperationContext()).ConfigureAwait(false);
                    await blob.FetchAttributesAsync(new AccessCondition(), _requestOptions, new OperationContext()).ConfigureAwait(false);
                }
                else if (blob.Metadata.Remove(_expiresMetadataKey))
                {
                    await blob.SetMetadataAsync(new AccessCondition(), _requestOptions, new OperationContext()).ConfigureAwait(false);
                    await blob.FetchAttributesAsync(new AccessCondition(), _requestOptions, new OperationContext()).ConfigureAwait(false);
                }

                return new BlobPutResult()
                {
                    BlobName = blobName,
                    DirectoryRelativeAddress = directoryRelativeAddress,
                    Status = PutBlobStatus.OK,
                    ContentType = blob.Properties.ContentType,
                    ContentEncoding = blob.Properties.ContentEncoding,
                    ETag = blob.Properties.ETag,
                    Created = blob.Properties.Created,
                    LastModified = blob.Properties.LastModified,
                    Expires = absoluteExpiration
                };
            }
            catch (StorageException ex)
            {
                // preconditions failed, optimistic concurrency check failed
                if (ex.RequestInformation.HttpStatusCode == 412)
                {
                    return new BlobPutResult()
                    {
                        BlobName = blobName,
                        DirectoryRelativeAddress = directoryRelativeAddress,
                        Status = PutBlobStatus.PreconditionFailed,
                        ContentType = blob.Properties.ContentType,
                        ContentEncoding = blob.Properties.ContentEncoding,
                        ETag = blob.Properties.ETag,
                        Created = blob.Properties.Created,
                        LastModified = blob.Properties.LastModified,
                        Expires = absoluteExpiration
                    };
                }

                throw;
            }
            finally
            {
                try
                {
                    if (leaseId != null)
                    {
                        // do not pass cancellation token
                        await blob.ReleaseLeaseAsync(new AccessCondition() { LeaseId = leaseId }, _requestOptions, new OperationContext()).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // TODO: investigate what can happen here
                }
            }
        }

        public async Task<DeleteBlobStatus> DeleteAsync(string blobName, string directoryRelativeAddress = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            directoryRelativeAddress = directoryRelativeAddress ?? _defaultDirectoryRelativeAddress;
            ArgCheck.NotNull(nameof(directoryRelativeAddress), directoryRelativeAddress);

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

            var leaseId = (string)null;

            try
            {
                try
                {
                    if (_leaseDuration.HasValue)
                    {
                        leaseId = await blob.AcquireLeaseAsync(TimeSpan.FromSeconds(60), null, new AccessCondition(), _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (StorageException ex)
                {
                    // 404: not found
                    if (ex.RequestInformation.HttpStatusCode == 404)
                    {
                        return DeleteBlobStatus.NotFound;
                    }

                    throw;
                }

                try
                {
                    await blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, new AccessCondition() { IfMatchETag = ifMatchETag, IfNotModifiedSinceTime = ifNotModifiedSinceTime }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                }
                catch (StorageException ex)
                {
                    // 404: not found
                    if (ex.RequestInformation.HttpStatusCode == 404)
                    {
                        return DeleteBlobStatus.NotFound;
                    }

                    // 412: preconditions failed, optimistic concurrency check failed
                    if (ex.RequestInformation.HttpStatusCode == 412)
                    {
                        return DeleteBlobStatus.PreconditionFailed;
                    }

                    throw;
                }
            }
            finally
            {
                try
                {
                    if (leaseId != null)
                    {
                        // do not pass cancellation token
                        await blob.ReleaseLeaseAsync(new AccessCondition() { LeaseId = leaseId }, _requestOptions, new OperationContext()).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // TODO: investigate what can happen here
                }
            }

            return DeleteBlobStatus.OK;
        }

        public async Task CreateIfNotExistsAsync(BlobContainerPublicAccessType publicAccessType, CancellationToken cancellationToken = default)
        {
            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            await _container.CreateIfNotExistsAsync(publicAccessType, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveExpiredAsync(CancellationToken cancellationToken = default)
        {
            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var utcNow = DateTimeOffset.UtcNow;

            BlobResultSegment segment = null;
            while (segment == null || segment.ContinuationToken != null)
            {
                segment = await _container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, null, segment?.ContinuationToken, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                foreach (ICloudBlob blob in segment.Results)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // expired?
                    if (blob.Metadata.TryGetValue(_expiresMetadataKey, out var expirationString) &&
                        DateTimeOffset.TryParseExact(expirationString, _dateTimeFormatString, null, DateTimeStyles.None, out var expiration) &&
                        expiration <= utcNow)
                    {
                        await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, new AccessCondition() { IfMatchETag = blob.Properties.ETag }, _requestOptions, new OperationContext()).ConfigureAwait(false);
                    }
                }
            }
        }

        private CloudBlockBlob GetBlobReference(string blobName, string directoryRelativeAddress)
        {
            if (!string.IsNullOrEmpty(directoryRelativeAddress))
            {
                return _container.GetDirectoryReference(directoryRelativeAddress).GetBlockBlobReference(blobName);
            }

            return _container.GetBlockBlobReference(blobName);
        }

        async Task<IStringResult> IKeyValueReadOnlyStore.GetStringAsync(string key, string partition, object concurrencyStamp, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            return await GetStringAsync(key, partition, (string)concurrencyStamp, throwOnNotFound: throwOnNotFound, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IBytesResult> IKeyValueReadOnlyStore.GetBytesAsync(string key, string partition, object concurrencyStamp, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            return await GetBytesAsync(key, partition, (string)concurrencyStamp, throwOnNotFound: throwOnNotFound, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IStreamResult> IKeyValueReadOnlyStore.GetStreamAsync(string key, string partition, object concurrencyStamp, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            return await GetStreamAsync(key, partition, (string)concurrencyStamp, throwOnNotFound: throwOnNotFound, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<object> IKeyValueStore.AddOrReplaceAsync(string key, string value, string partition, object concurrencyStamp, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken)
        {
            return (await PutAsync(key, value, partition, (string)concurrencyStamp, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task<object> IKeyValueStore.AddOrReplaceAsync(string key, byte[] bytes, string partition, object concurrencyStamp, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken)
        {
            return (await PutAsync(key, bytes, partition, (string)concurrencyStamp, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task<object> IKeyValueStore.AddOrReplaceAsync(string key, Stream stream, string partition, object concurrencyStamp, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken)
        {
            return (await PutAsync(key, stream, partition, (string)concurrencyStamp, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task IKeyValueStore.RemoveAsync(string key, string partition, CancellationToken cancellationToken)
        {
            await DeleteAsync(key, partition, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<X509Certificate2> IPublicCertificateStore.GetPublicCertificateAsync(string name, CancellationToken cancellationToken)
        {
            var result = await GetBytesAsync(name, throwOnNotFound: true, cancellationToken: cancellationToken).ConfigureAwait(false);
            return new X509Certificate2(result.Contents);
        }
    }
}

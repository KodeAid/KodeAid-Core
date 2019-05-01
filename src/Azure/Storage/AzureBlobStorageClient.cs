// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public class AzureBlobStorageClient : IDataStore, IKeyValueStore, ISharedUriAccessible, IPublicCertificateStore, IInitializableAsync
    {
        private const string _expiresMetadataKey = "Expires";
        private const string _dateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        private readonly string _defaultDirectoryRelativeAddress;
        private readonly ISecretReadOnlyStore _secretStore;
        private readonly string _connectionStringSecretName;
        private readonly string _accountKeySecretName;
        private readonly string _sharedAccessSignatureSecretName;
        private readonly string _containerName;
        private readonly string _accountName;
        private readonly string _endpointSuffix;
        private CloudBlobContainer _container;
        private readonly TimeSpan? _leaseDuration;
        private readonly bool _useSnapshots;
        private readonly BlobRequestOptions _requestOptions = new BlobRequestOptions() { RetryPolicy = new ExponentialRetry() }; //EncryptionPolicy = new TableEncryptionPolicy()
        private readonly BlobRequestOptions _requestOptionsWithoutRetry = new BlobRequestOptions() { }; //EncryptionPolicy = new TableEncryptionPolicy()
        private readonly bool _deleteExpiredDuringRequests = false;

        public AzureBlobStorageClient(AzureBlobStorageClientOptions options)
        {
            ArgCheck.NotNull(nameof(options), options);

            options.Verify();

            if (options.StorageAccount != null)
            {
                _container = options.StorageAccount.CreateCloudBlobClient().GetContainerReference(options.ContainerName);
            }
            else
            {
                _secretStore = options.SecretStore;
                _connectionStringSecretName = options.ConnectionStringSecretName;
                _accountKeySecretName = options.AccountKeySecretName;
                _sharedAccessSignatureSecretName = options.SharedAccessSignatureSecretName;
                _accountName = options.AccountName;
                _endpointSuffix = options.EndpointSuffix;
                _containerName = options.ContainerName;
            }

            _defaultDirectoryRelativeAddress = options.DefaultDirectoryRelativeAddress;
            _leaseDuration = options.LeaseDuration;
            _useSnapshots = options.UseSnapshots;
        }

        public bool IsInitialized => _container != null;

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
            {
                return;
            }

            var secret = await _secretStore.GetSecretAsync(_connectionStringSecretName ?? _accountKeySecretName ?? _sharedAccessSignatureSecretName, cancellationToken).ConfigureAwait(false);
            if (_connectionStringSecretName != null)
            {
                _container = CloudStorageAccount.Parse(secret.Unsecure()).CreateCloudBlobClient().GetContainerReference(_containerName);
            }
            else if (_accountKeySecretName != null)
            {
                _container = new CloudStorageAccount(new StorageCredentials(_accountName, secret.Unsecure()), _accountName, _endpointSuffix ?? "core.windows.net", true).CreateCloudBlobClient().GetContainerReference(_containerName);
            }
            else
            {
                _container = new CloudStorageAccount(new StorageCredentials(secret.Unsecure()), _accountName, _endpointSuffix ?? "core.windows.net", true).CreateCloudBlobClient().GetContainerReference(_containerName);
            }
        }

        public async Task<bool> ExistsAsync(string blobName, string directoryRelativeAddress = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);

            directoryRelativeAddress = directoryRelativeAddress ?? _defaultDirectoryRelativeAddress;

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

            return await blob.ExistsAsync(_requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BlobResult>> ListAsync(string directoryRelativeAddress = null, CancellationToken cancellationToken = default)
        {
            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var utcNow = DateTimeOffset.UtcNow;

            var results = new List<BlobResult>();

            BlobResultSegment segment = null;
            while (segment == null || segment.ContinuationToken != null)
            {
                segment = await _container.ListBlobsSegmentedAsync(directoryRelativeAddress, true, BlobListingDetails.Metadata, null, segment?.ContinuationToken, _requestOptionsWithoutRetry, new OperationContext(), cancellationToken).ConfigureAwait(false);
                foreach (ICloudBlob blob in segment.Results)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // skip expired
                    DateTimeOffset? expires = null;
                    if (blob.Metadata.TryGetValue(_expiresMetadataKey, out var expirationString) &&
                        DateTimeOffset.TryParseExact(expirationString, _dateTimeFormatString, null, DateTimeStyles.None, out var expiration))
                    {
                        expires = expiration;
                        if (expiration <= utcNow)
                        {
                            continue;
                        }
                    }

                    var result = new BlobResult()
                    {
                        BlobName = Path.GetFileName(blob.Name),
                        ContentEncoding = blob.Properties.ContentEncoding,
                        ContentType = blob.Properties.ContentType,
                        Created = blob.Properties.Created,
                        DirectoryRelativeAddress = Path.GetDirectoryName(blob.Name)?.Trim('/').TrimToNull(),
                        ETag = blob.Properties.ETag,
                        Expires = expires,
                        LastModified = blob.Properties.LastModified,
                    };
                    result.Metadata.AddRange(blob.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));
                    results.Add(result);
                }
            }

            return results;
        }

        public async Task<BlobStringResult> GetStringAsync(string blobName, string directoryRelativeAddress = null, string ifNoneMatchETag = null, DateTimeOffset? ifModifiedSinceTime = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default)
        {
            using (var result = await GetStreamAsync(blobName, directoryRelativeAddress, ifNoneMatchETag, ifModifiedSinceTime, throwOnNotFound, cancellationToken).ConfigureAwait(false))
            {
                if (result.Status != GetBlobStatus.OK)
                {
                    return new BlobStringResult(result);
                }

                using (result.Content)
                using (var ms = new MemoryStream())
                {
                    await result.Content.CopyToAsync(ms, 81920, cancellationToken).ConfigureAwait(false);
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

                using (result.Content)
                using (var ms = new MemoryStream())
                {
                    await result.Content.CopyToAsync(ms, 81920, cancellationToken).ConfigureAwait(false);
                    await ms.FlushAsync(cancellationToken).ConfigureAwait(false);

                    return new BlobBytesResult(result, ms.ToArray());
                }
            }
        }

        public async Task<BlobStreamResult> GetStreamAsync(string blobName, string directoryRelativeAddress = null, string ifNoneMatchETag = null, DateTimeOffset? ifModifiedSinceTime = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            directoryRelativeAddress = directoryRelativeAddress ?? _defaultDirectoryRelativeAddress;

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

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
                var result = new BlobStreamResult()
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
                result.Metadata.AddRange(blob.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));
                return result;
            }

            if (ifModifiedSinceTime.HasValue && blob.Properties.LastModified.HasValue && blob.Properties.LastModified <= ifModifiedSinceTime)
            {
                var result = new BlobStreamResult()
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
                result.Metadata.AddRange(blob.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));
                return result;
            }

            try
            {
                var stream = await blob.OpenReadAsync(new AccessCondition() { IfNoneMatchETag = ifNoneMatchETag, IfModifiedSinceTime = ifModifiedSinceTime }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);

                var result = new BlobStreamResult(stream)
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
                result.Metadata.AddRange(blob.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));
                return result;
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
                    var result = new BlobStreamResult()
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
                    result.Metadata.AddRange(blob.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));
                    return result;
                }

                throw;
            }
        }

        public Task<BlobResult> PutAsync(string blobName, string content, string directoryRelativeAddress = null, string contentType = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            return PutAsync(blobName, content, Encoding.UTF8, directoryRelativeAddress, contentType, ifMatchETag, ifNotModifiedSinceTime, absoluteExpiration, cancellationToken);
        }

        public Task<BlobResult> PutAsync(string blobName, string content, Encoding encoding, string directoryRelativeAddress = null, string contentType = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            ArgCheck.NotNull(nameof(content), content);

            encoding = encoding ?? Encoding.UTF8;

            return PutAsync(blobName, encoding.GetBytes(content), directoryRelativeAddress, contentType, encoding.WebName, ifMatchETag, ifNotModifiedSinceTime, absoluteExpiration, cancellationToken);
        }

        public async Task<BlobResult> PutAsync(string blobName, byte[] content, string directoryRelativeAddress = null, string contentType = null, string contentEncoding = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            ArgCheck.NotNull(nameof(content), content);

            using (var ms = new MemoryStream(content, false))
            {
                ms.Position = 0;
                return await PutAsync(blobName, ms, directoryRelativeAddress, contentType, contentEncoding, ifMatchETag, ifNotModifiedSinceTime, absoluteExpiration, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<BlobResult> PutAsync(string blobName, Stream content, string directoryRelativeAddress = null, string contentType = null, string contentEncoding = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            ArgCheck.NotNull(nameof(content), content);
            directoryRelativeAddress = directoryRelativeAddress ?? _defaultDirectoryRelativeAddress;

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

            var leaseId = (string)null;

            try
            {
                var exists = await blob.ExistsAsync(_requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);

                if (exists)
                {
                    if (_leaseDuration.HasValue)
                    {
                        leaseId = await blob.AcquireLeaseAsync(_leaseDuration.Value, null, new AccessCondition(), _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                    }

                    if (_useSnapshots)
                    {
                        await blob.CreateSnapshotAsync(null, new AccessCondition() { LeaseId = leaseId, IfMatchETag = ifMatchETag, IfNotModifiedSinceTime = ifNotModifiedSinceTime }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                    }
                }

                await blob.UploadFromStreamAsync(content, new AccessCondition() { LeaseId = leaseId, IfMatchETag = ifMatchETag, IfNotModifiedSinceTime = ifNotModifiedSinceTime }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);

                if (leaseId == null && _leaseDuration.HasValue)
                {
                    // if it was created (not updated) then lets get a lease to the new blob
                    leaseId = await blob.AcquireLeaseAsync(_leaseDuration.Value, null, new AccessCondition(), _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                }

                // load metadata and properties
                await blob.FetchAttributesAsync(new AccessCondition() { LeaseId = leaseId }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);

                var propertiesChanged = false;

                if (!string.IsNullOrWhiteSpace(contentType) && blob.Properties.ContentType != contentType)
                {
                    blob.Properties.ContentType = contentType;
                    propertiesChanged = true;
                }

                if (!string.IsNullOrWhiteSpace(contentEncoding) && blob.Properties.ContentEncoding != contentEncoding)
                {
                    blob.Properties.ContentEncoding = contentEncoding;
                    propertiesChanged = true;
                }

                if (propertiesChanged)
                {
                    await blob.SetPropertiesAsync(new AccessCondition() { LeaseId = leaseId }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                }

                if (absoluteExpiration.HasValue)
                {
                    // set expiration
                    blob.Metadata[_expiresMetadataKey] = absoluteExpiration.Value.ToString(_dateTimeFormatString);
                    await blob.SetMetadataAsync(new AccessCondition() { LeaseId = leaseId }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                    await blob.FetchAttributesAsync(new AccessCondition() { LeaseId = leaseId }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                }
                else if (blob.Metadata.Remove(_expiresMetadataKey))
                {
                    await blob.SetMetadataAsync(new AccessCondition() { LeaseId = leaseId }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                    await blob.FetchAttributesAsync(new AccessCondition() { LeaseId = leaseId }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                }

                var result = new BlobPutResult()
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
                result.Metadata.AddRange(blob.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));
                return result;
            }
            catch (StorageException ex)
            {
                // preconditions failed, optimistic concurrency check failed
                if (ex.RequestInformation.HttpStatusCode == 412)
                {
                    var result = new BlobPutResult()
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
                    result.Metadata.AddRange(blob.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));
                    return result;
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
                        await blob.ReleaseLeaseAsync(new AccessCondition() { LeaseId = leaseId }, cancellationToken.IsCancellationRequested ? _requestOptionsWithoutRetry : _requestOptions, new OperationContext()).ConfigureAwait(false);
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

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

            var leaseId = (string)null;

            try
            {
                try
                {
                    if (_leaseDuration.HasValue)
                    {
                        leaseId = await blob.AcquireLeaseAsync(_leaseDuration.Value, null, new AccessCondition(), _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                    }

                    await blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, new AccessCondition() { LeaseId = leaseId, IfMatchETag = ifMatchETag, IfNotModifiedSinceTime = ifNotModifiedSinceTime }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
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
                        await blob.ReleaseLeaseAsync(new AccessCondition() { LeaseId = leaseId }, cancellationToken.IsCancellationRequested ? _requestOptionsWithoutRetry : _requestOptions, new OperationContext()).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // TODO: investigate what can happen here
                }
            }

            return DeleteBlobStatus.OK;
        }

        public async Task<SnapshotBlobStatus> SnapshopAsync(string blobName, string directoryRelativeAddress = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            directoryRelativeAddress = directoryRelativeAddress ?? _defaultDirectoryRelativeAddress;

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

            var leaseId = (string)null;

            try
            {
                try
                {
                    if (_leaseDuration.HasValue)
                    {
                        leaseId = await blob.AcquireLeaseAsync(_leaseDuration.Value, null, new AccessCondition(), _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                    }

                    await blob.CreateSnapshotAsync(null, new AccessCondition() { LeaseId = leaseId, IfMatchETag = ifMatchETag, IfNotModifiedSinceTime = ifNotModifiedSinceTime }, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
                }
                catch (StorageException ex)
                {
                    // 404: not found
                    if (ex.RequestInformation.HttpStatusCode == 404)
                    {
                        return SnapshotBlobStatus.NotFound;
                    }

                    // 412: preconditions failed, optimistic concurrency check failed
                    if (ex.RequestInformation.HttpStatusCode == 412)
                    {
                        return SnapshotBlobStatus.PreconditionFailed;
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
                        await blob.ReleaseLeaseAsync(new AccessCondition() { LeaseId = leaseId }, cancellationToken.IsCancellationRequested ? _requestOptionsWithoutRetry : _requestOptions, new OperationContext()).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // TODO: investigate what can happen here
                }
            }

            return SnapshotBlobStatus.OK;
        }

        public async Task<Uri> GetSharedAccessUri(string blobName, string directoryRelativeAddress = null, SharedAccessBlobPermissions permissions = SharedAccessBlobPermissions.Read, DateTimeOffset? sharedAccessStartTime = null, DateTimeOffset? sharedAccessExpiryTime = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            directoryRelativeAddress = directoryRelativeAddress ?? _defaultDirectoryRelativeAddress;

            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var blob = GetBlobReference(blobName, directoryRelativeAddress);

            var policy = new SharedAccessBlobPolicy
            {
                SharedAccessStartTime = sharedAccessStartTime,
                SharedAccessExpiryTime = sharedAccessExpiryTime,
                Permissions = permissions,
            };

            var sas = blob.GetSharedAccessSignature(policy, null, null, SharedAccessProtocol.HttpsOnly, null);

            return new Uri(blob.Uri + sas);
        }

        public async Task CreateIfNotExistsAsync(BlobContainerPublicAccessType publicAccessType, CancellationToken cancellationToken = default)
        {
            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            await _container.CreateIfNotExistsAsync(publicAccessType, _requestOptions, new OperationContext(), cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveExpiredAsync(string directoryRelativeAddress = null, CancellationToken cancellationToken = default)
        {
            await InitializeAsync(cancellationToken).ConfigureAwait(false);

            var utcNow = DateTimeOffset.UtcNow;

            BlobResultSegment segment = null;
            while (segment == null || segment.ContinuationToken != null)
            {
                segment = await _container.ListBlobsSegmentedAsync(directoryRelativeAddress, true, BlobListingDetails.Metadata, null, segment?.ContinuationToken, _requestOptionsWithoutRetry, new OperationContext(), cancellationToken).ConfigureAwait(false);
                foreach (ICloudBlob blob in segment.Results)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // expired?
                    if (blob.Metadata.TryGetValue(_expiresMetadataKey, out var expirationString) &&
                        DateTimeOffset.TryParseExact(expirationString, _dateTimeFormatString, null, DateTimeStyles.None, out var expiration) &&
                        expiration <= utcNow)
                    {
                        try
                        {
                            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, new AccessCondition() { IfMatchETag = blob.Properties.ETag }, _requestOptionsWithoutRetry, new OperationContext()).ConfigureAwait(false);
                        }
                        catch (StorageException ex)
                        {
                            // 404: not found
                            if (ex.RequestInformation.HttpStatusCode == 404)
                            {
                                continue;
                            }

                            // 412: preconditions failed, optimistic concurrency check failed
                            if (ex.RequestInformation.HttpStatusCode == 412)
                            {
                                continue;
                            }

                            throw;
                        }
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

        async Task<bool> IDataStore.ExistsAsync(string name, string partition, CancellationToken cancellationToken)
        {
            return await ExistsAsync(name, partition, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IEnumerable<IBlobMeta>> IDataStore.ListAsync(string partition, CancellationToken cancellationToken)
        {
            return await ListAsync(partition, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IBlobResult> IDataStore.GetAsync(string name, string partition, object concurrencyStamp, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            return await GetStreamAsync(name, partition, (string)concurrencyStamp, throwOnNotFound: throwOnNotFound, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<object> IDataStore.AddOrReplaceAsync(BlobData blob, CancellationToken cancellationToken)
        {
            return (await PutAsync(blob?.Key, blob?.Content, directoryRelativeAddress: blob?.Partition, contentType: blob?.ContentType, contentEncoding: blob?.ContentEncoding, ifMatchETag: (string)blob?.ConcurrencyStamp, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task IDataStore.RemoveAsync(string key, string partition, CancellationToken cancellationToken)
        {
            await DeleteAsync(key, partition, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<Uri> ISharedUriAccessible.GetSharedUri(string name, string partition, AccessPermissions permissions, DateTimeOffset? startTime, DateTimeOffset? expiryTime, CancellationToken cancellationToken)
        {
            var sharedAccessBlobPermissions = SharedAccessBlobPermissions.None;

            if (permissions.HasFlagSet(AccessPermissions.Read))
            {
                sharedAccessBlobPermissions |= SharedAccessBlobPermissions.Read;
            }

            if (permissions.HasFlagSet(AccessPermissions.Write))
            {
                sharedAccessBlobPermissions |= SharedAccessBlobPermissions.Write;
            }

            if (permissions.HasFlagSet(AccessPermissions.Delete))
            {
                sharedAccessBlobPermissions |= SharedAccessBlobPermissions.Delete;
            }

            return await GetSharedAccessUri(name, partition, sharedAccessBlobPermissions, startTime, expiryTime, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            return (await PutAsync(key, value, directoryRelativeAddress: partition, ifMatchETag: (string)concurrencyStamp, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task<object> IKeyValueStore.AddOrReplaceAsync(string key, byte[] bytes, string partition, object concurrencyStamp, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken)
        {
            return (await PutAsync(key, bytes, directoryRelativeAddress: partition, ifMatchETag: (string)concurrencyStamp, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task<object> IKeyValueStore.AddOrReplaceAsync(string key, Stream stream, string partition, object concurrencyStamp, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken)
        {
            return (await PutAsync(key, stream, directoryRelativeAddress: partition, ifMatchETag: (string)concurrencyStamp, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
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

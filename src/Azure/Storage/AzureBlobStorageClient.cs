// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using KodeAid.Security.Cryptography.X509Certificates;
using KodeAid.Storage;

namespace KodeAid.Azure.Storage
{
    public class AzureBlobStorageClient : IDataStore, IKeyValueStore, ISharedUriAccessible, IPublicCertificateStore
    {
        private const string _expiresMetadataKey = "Expires";
        private const string _dateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        private readonly TimeSpan _defaultSharedAccessDuration = TimeSpan.FromMinutes(60);
        private readonly string _defaultDirectoryRelativeAddress;
        private readonly BlobServiceClient _accountClient;
        private readonly BlobContainerClient _containerClient;
        private readonly TimeSpan? _leaseDuration;
        private readonly bool _useSnapshots;
        private readonly StorageSharedKeyCredential _storageSharedKeyCredential;
        private readonly bool _useManagedIdentity;
        private readonly BlobClientOptions _clientOptions = new BlobClientOptions()
        {
            Retry =
            {
                Mode = RetryMode.Exponential,
                MaxRetries = 5,
                Delay = TimeSpan.FromSeconds(5),
                MaxDelay = TimeSpan.FromSeconds(10),
            }
        };
        private readonly bool _deleteExpiredDuringRequests = false;

        public AzureBlobStorageClient(AzureBlobStorageClientOptions options)
        {
            ArgCheck.NotNull(nameof(options), options);
            ArgCheck.NotNullOrEmpty(nameof(options.ContainerName), options.ContainerName);

            if (options.LeaseDuration.HasValue)
            {
                // As per Azure storage lease duration constraints.
                ArgCheck.GreaterThanOrEqualTo(nameof(options.LeaseDuration), options.LeaseDuration, TimeSpan.FromSeconds(15));
                ArgCheck.LessThanOrEqualTo(nameof(options.LeaseDuration), options.LeaseDuration, TimeSpan.FromSeconds(60));
            }

            if (options.NetworkTimeout.HasValue)
            {
                _clientOptions.Retry.NetworkTimeout = options.NetworkTimeout.Value;
            }

            if (!string.IsNullOrEmpty(options.ConnectionString))
            {
                _accountClient = new BlobServiceClient(options.ConnectionString, _clientOptions);
            }
            else if (!string.IsNullOrEmpty(options.AccountName))
            {
                var serviceUri = new Uri($"https://{options.AccountName}.{(options.EndpointSuffix ?? "blob.core.windows.net")}");

                if (!string.IsNullOrEmpty(options.AccountKey))
                {
                    _storageSharedKeyCredential = new StorageSharedKeyCredential(options.AccountName, options.AccountKey);
                    _accountClient = new BlobServiceClient(serviceUri, _storageSharedKeyCredential, _clientOptions);
                }
                else if (options.UseDefaultAzureCredential)
                {
                    _useManagedIdentity = true;
                    _accountClient = new BlobServiceClient(serviceUri, new DefaultAzureCredential(), _clientOptions);
                }
                else if (!string.IsNullOrEmpty(options.SharedAccessSignature))
                {
                    throw new NotSupportedException($"{nameof(options.SharedAccessSignature)} is not supported.");
                }
                else
                {
                    throw new ArgumentException("Account key, SAS or managed identity is required.", nameof(options));
                }
            }
            else
            {
                throw new ArgumentException("Connection string or account name is required.", nameof(options));
            }

            _containerClient = _accountClient.GetBlobContainerClient(options.ContainerName);
            _defaultDirectoryRelativeAddress = options.DefaultDirectoryRelativeAddress;
            _leaseDuration = options.LeaseDuration;
            _useSnapshots = options.UseSnapshots;
        }

        public async Task<bool> ExistsAsync(string blobName, string directoryRelativeAddress = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);

            directoryRelativeAddress ??= _defaultDirectoryRelativeAddress;
            var blob = GetBlobClient(blobName, directoryRelativeAddress);
            var response = await blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
            return response.Value;
        }

        public async Task<IEnumerable<BlobResult>> ListAsync(string directoryRelativeAddress = null, CancellationToken cancellationToken = default)
        {
            directoryRelativeAddress ??= _defaultDirectoryRelativeAddress;
            var utcNow = DateTimeOffset.UtcNow;
            var results = new List<BlobResult>();

            await foreach (var blob in _containerClient.GetBlobsAsync(BlobTraits.Metadata, BlobStates.None, directoryRelativeAddress, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // skip expired
                if (blob.Properties.ExpiresOn.HasValue && blob.Properties.ExpiresOn.Value <= utcNow)
                {
                    continue;
                }

                var result = new BlobResult()
                {
                    BlobName = Path.GetFileName(blob.Name),
                    ContentEncoding = blob.Properties.ContentEncoding,
                    ContentType = blob.Properties.ContentType,
                    CreatedOn = blob.Properties.CreatedOn,
                    DirectoryRelativeAddress = Path.GetDirectoryName(blob.Name)?.Trim('/').TrimToNull(),
                    ETag = blob.Properties.ETag?.ToString("G"),
                    Expires = blob.Properties.ExpiresOn,
                    LastModified = blob.Properties.LastModified,
                };

                result.Metadata.AddRange(blob.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));

                results.Add(result);
            }

            return results;
        }

        public async Task<BlobStringResult> GetStringAsync(string blobName, string directoryRelativeAddress = null, string ifNoneMatchETag = null, DateTimeOffset? ifModifiedSinceTime = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default)
        {
            using var result = await GetStreamAsync(blobName, directoryRelativeAddress, ifNoneMatchETag, ifModifiedSinceTime, throwOnNotFound, cancellationToken).ConfigureAwait(false);

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

        public async Task<BlobBytesResult> GetBytesAsync(string blobName, string directoryRelativeAddress = null, string ifNoneMatchETag = null, DateTimeOffset? ifModifiedSinceTime = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default)
        {
            using var result = await GetStreamAsync(blobName, directoryRelativeAddress, ifNoneMatchETag, ifModifiedSinceTime, throwOnNotFound, cancellationToken).ConfigureAwait(false);

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

        public async Task<BlobStreamResult> GetStreamAsync(string blobName, string directoryRelativeAddress = null, string ifNoneMatchETag = null, DateTimeOffset? ifModifiedSinceTime = null, bool throwOnNotFound = false, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);

            directoryRelativeAddress ??= _defaultDirectoryRelativeAddress;
            var blobClient = GetBlobClient(blobName, directoryRelativeAddress);
            BlobProperties blobProperties = null;

            try
            {
                // load metadata and properties
                blobProperties = (await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).Value;
            }
            catch (RequestFailedException ex)
            {
                // 404: not found
                if (!throwOnNotFound && ex.Status == 404)
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
            if (blobProperties.ExpiresOn != default && blobProperties.ExpiresOn <= DateTimeOffset.UtcNow)
            {
                if (_deleteExpiredDuringRequests)
                {
                    try
                    {
                        // remove if expired
                        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, new BlobRequestConditions() { IfMatch = blobProperties.ETag }, cancellationToken).ConfigureAwait(false);
                    }
                    catch (RequestFailedException)
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

            if (ifNoneMatchETag != null && blobProperties.ETag != default && blobProperties.ETag.ToString("G") == ifNoneMatchETag)
            {
                var result = new BlobStreamResult()
                {
                    BlobName = blobName,
                    DirectoryRelativeAddress = directoryRelativeAddress,
                    Status = GetBlobStatus.NotModified,
                    ContentType = blobProperties.ContentType?.TrimToNull(),
                    ContentEncoding = blobProperties.ContentEncoding?.TrimToNull(),
                    ETag = blobProperties.ETag.ToString("G")?.TrimToNull(),
                    CreatedOn = blobProperties.CreatedOn == default || blobProperties.CreatedOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.CreatedOn,
                    LastModified = blobProperties.LastModified == default || blobProperties.LastModified == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.LastModified,
                    Expires = blobProperties.ExpiresOn == default || blobProperties.ExpiresOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.ExpiresOn,
                };

                result.Metadata.AddRange(blobProperties.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));

                return result;
            }

            if (ifModifiedSinceTime.HasValue && blobProperties.LastModified != default && blobProperties.LastModified <= ifModifiedSinceTime)
            {
                var result = new BlobStreamResult()
                {
                    BlobName = blobName,
                    DirectoryRelativeAddress = directoryRelativeAddress,
                    Status = GetBlobStatus.NotModified,
                    ContentType = blobProperties.ContentType?.TrimToNull(),
                    ContentEncoding = blobProperties.ContentEncoding?.TrimToNull(),
                    ETag = blobProperties.ETag.ToString("G")?.TrimToNull(),
                    CreatedOn = blobProperties.CreatedOn == default || blobProperties.CreatedOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.CreatedOn,
                    LastModified = blobProperties.LastModified == default || blobProperties.LastModified == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.LastModified,
                    Expires = blobProperties.ExpiresOn == default || blobProperties.ExpiresOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.ExpiresOn,
                };

                result.Metadata.AddRange(blobProperties.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));

                return result;
            }

            try
            {
                var stream = await blobClient.OpenReadAsync(new BlobOpenReadOptions(true) { Conditions = new BlobRequestConditions() { IfNoneMatch = ifNoneMatchETag != null ? new ETag(ifNoneMatchETag) : (ETag?)null, IfModifiedSince = ifModifiedSinceTime } }, cancellationToken).ConfigureAwait(false);

                var result = new BlobStreamResult(stream)
                {
                    BlobName = blobName,
                    DirectoryRelativeAddress = directoryRelativeAddress,
                    Status = GetBlobStatus.OK,
                    ContentType = blobProperties.ContentType?.TrimToNull(),
                    ContentEncoding = blobProperties.ContentEncoding?.TrimToNull(),
                    ETag = blobProperties.ETag.ToString("G")?.TrimToNull(),
                    CreatedOn = blobProperties.CreatedOn == default || blobProperties.CreatedOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.CreatedOn,
                    LastModified = blobProperties.LastModified == default || blobProperties.LastModified == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.LastModified,
                    Expires = blobProperties.ExpiresOn == default || blobProperties.ExpiresOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.ExpiresOn,
                };

                result.Metadata.AddRange(blobProperties.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));

                return result;
            }
            catch (RequestFailedException ex)
            {
                // 404: not found
                if (!throwOnNotFound && ex.Status == 404)
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
                if (ex.Status == 304 || ex.Status == 412)
                {
                    var result = new BlobStreamResult()
                    {
                        BlobName = blobName,
                        DirectoryRelativeAddress = directoryRelativeAddress,
                        Status = GetBlobStatus.NotModified,
                        ContentType = blobProperties.ContentType?.TrimToNull(),
                        ContentEncoding = blobProperties.ContentEncoding?.TrimToNull(),
                        ETag = blobProperties.ETag.ToString("G")?.TrimToNull(),
                        CreatedOn = blobProperties.CreatedOn == default || blobProperties.CreatedOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.CreatedOn,
                        LastModified = blobProperties.LastModified == default || blobProperties.LastModified == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.LastModified,
                        Expires = blobProperties.ExpiresOn == default || blobProperties.ExpiresOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.ExpiresOn,
                    };

                    result.Metadata.AddRange(blobProperties.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));

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

            encoding ??= Encoding.UTF8;

            return PutAsync(blobName, encoding.GetBytes(content), directoryRelativeAddress, contentType, encoding.WebName, ifMatchETag, ifNotModifiedSinceTime, absoluteExpiration, cancellationToken);
        }

        public async Task<BlobResult> PutAsync(string blobName, byte[] content, string directoryRelativeAddress = null, string contentType = null, string contentEncoding = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            ArgCheck.NotNull(nameof(content), content);

            using var ms = new MemoryStream(content, false);
            ms.Position = 0;
            return await PutAsync(blobName, ms, directoryRelativeAddress, contentType, contentEncoding, ifMatchETag, ifNotModifiedSinceTime, absoluteExpiration, cancellationToken).ConfigureAwait(false);
        }

        public async Task<BlobResult> PutAsync(string blobName, Stream content, string directoryRelativeAddress = null, string contentType = null, string contentEncoding = null, string ifMatchETag = null, DateTimeOffset? ifNotModifiedSinceTime = null, DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);
            ArgCheck.NotNull(nameof(content), content);

            directoryRelativeAddress ??= _defaultDirectoryRelativeAddress;
            var blobClient = GetBlobClient(blobName, directoryRelativeAddress);
            BlobProperties blobProperties = null;
            var leaseClient = new BlobLeaseClient(blobClient);
            string leaseId = null;

            try
            {
                var exists = await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);

                if (exists)
                {
                    if (_leaseDuration.HasValue)
                    {
                        leaseId = (await leaseClient.AcquireAsync(_leaseDuration.Value, new RequestConditions(), cancellationToken).ConfigureAwait(false)).Value.LeaseId;
                    }

                    blobProperties = (await blobClient.GetPropertiesAsync(new BlobRequestConditions() { LeaseId = leaseId }, cancellationToken).ConfigureAwait(false)).Value;

                    if (_useSnapshots)
                    {
                        await blobClient.CreateSnapshotAsync(null, new BlobRequestConditions() { LeaseId = leaseId, IfMatch = ifMatchETag != null ? new ETag(ifMatchETag) : (ETag?)null, IfUnmodifiedSince = ifNotModifiedSinceTime }, cancellationToken).ConfigureAwait(false);
                    }
                }

                var uploadOptions = new BlobUploadOptions()
                {
                    Conditions = new BlobRequestConditions()
                    {
                        LeaseId = leaseId,
                        IfMatch = ifMatchETag != null ? new ETag(ifMatchETag) : (ETag?)null,
                        IfUnmodifiedSince = ifNotModifiedSinceTime
                    },
                    HttpHeaders = new BlobHttpHeaders()
                    {
                        ContentType = contentType,
                        ContentEncoding = contentEncoding,
                    }
                };

                if (absoluteExpiration.HasValue)
                {
                    uploadOptions.Metadata = new Dictionary<string, string>()
                    {
                        { _expiresMetadataKey, absoluteExpiration.Value.ToString(_dateTimeFormatString) }
                    };
                }

                var blobInfo = (await blobClient.UploadAsync(content, uploadOptions, cancellationToken).ConfigureAwait(false)).Value;
                blobProperties = (await blobClient.GetPropertiesAsync(new BlobRequestConditions() { LeaseId = leaseId }, cancellationToken).ConfigureAwait(false)).Value;

                var result = new BlobPutResult()
                {
                    BlobName = blobName,
                    DirectoryRelativeAddress = directoryRelativeAddress,
                    Status = PutBlobStatus.OK,
                    ContentType = blobProperties.ContentType?.TrimToNull(),
                    ContentEncoding = blobProperties.ContentEncoding?.TrimToNull(),
                    ETag = blobInfo.ETag.ToString("G")?.TrimToNull(),
                    CreatedOn = blobProperties.CreatedOn == default || blobProperties.CreatedOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.CreatedOn,
                    LastModified = blobInfo.LastModified == default || blobInfo.LastModified == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobInfo.LastModified,
                    Expires = blobProperties.ExpiresOn == default || blobProperties.ExpiresOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.ExpiresOn,
                };

                result.Metadata.AddRange(blobProperties.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));

                return result;
            }
            catch (RequestFailedException ex)
            {
                // preconditions failed, optimistic concurrency check failed
                if (ex.Status == 412)
                {
                    var result = new BlobPutResult()
                    {
                        BlobName = blobName,
                        DirectoryRelativeAddress = directoryRelativeAddress,
                        Status = PutBlobStatus.PreconditionFailed,
                        ContentType = blobProperties.ContentType?.TrimToNull(),
                        ContentEncoding = blobProperties.ContentEncoding?.TrimToNull(),
                        ETag = blobProperties.ETag.ToString("G")?.TrimToNull(),
                        CreatedOn = blobProperties.CreatedOn == default || blobProperties.CreatedOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.CreatedOn,
                        LastModified = blobProperties.LastModified == default || blobProperties.LastModified == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.LastModified,
                        Expires = blobProperties.ExpiresOn == default || blobProperties.ExpiresOn == DateTimeOffset.MaxValue ? (DateTimeOffset?)null : blobProperties.ExpiresOn,
                    };

                    result.Metadata.AddRange(blobProperties.Metadata.Where(p => !string.Equals(p.Key, _expiresMetadataKey, StringComparison.OrdinalIgnoreCase)));

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
                        await leaseClient.ReleaseAsync(new RequestConditions()).ConfigureAwait(false);
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

            directoryRelativeAddress ??= _defaultDirectoryRelativeAddress;
            var blobClient = GetBlobClient(blobName, directoryRelativeAddress);
            var leaseClient = new BlobLeaseClient(blobClient);
            string leaseId = null;

            try
            {
                try
                {
                    if (_leaseDuration.HasValue)
                    {
                        leaseId = (await leaseClient.AcquireAsync(_leaseDuration.Value, new RequestConditions(), cancellationToken).ConfigureAwait(false)).Value.LeaseId;
                    }

                    await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, new BlobRequestConditions() { LeaseId = leaseId, IfMatch = ifMatchETag != null ? new ETag(ifMatchETag) : (ETag?)null, IfUnmodifiedSince = ifNotModifiedSinceTime }, cancellationToken).ConfigureAwait(false);
                }
                catch (RequestFailedException ex)
                {
                    // 404: not found
                    if (ex.Status == 404)
                    {
                        return DeleteBlobStatus.NotFound;
                    }

                    // 412: preconditions failed, optimistic concurrency check failed
                    if (ex.Status == 412)
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
                        await leaseClient.ReleaseAsync(new RequestConditions()).ConfigureAwait(false);
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

            directoryRelativeAddress ??= _defaultDirectoryRelativeAddress;
            var blobClient = GetBlobClient(blobName, directoryRelativeAddress);
            var leaseClient = new BlobLeaseClient(blobClient);
            string leaseId = null;

            try
            {
                try
                {
                    if (_leaseDuration.HasValue)
                    {
                        leaseId = (await leaseClient.AcquireAsync(_leaseDuration.Value, new RequestConditions(), cancellationToken).ConfigureAwait(false)).Value.LeaseId;
                    }

                    await blobClient.CreateSnapshotAsync(null, new BlobRequestConditions() { LeaseId = leaseId, IfMatch = ifMatchETag != null ? new ETag(ifMatchETag) : (ETag?)null, IfUnmodifiedSince = ifNotModifiedSinceTime }, cancellationToken).ConfigureAwait(false);
                }
                catch (RequestFailedException ex)
                {
                    // 404: not found
                    if (ex.Status == 404)
                    {
                        return SnapshotBlobStatus.NotFound;
                    }

                    // 412: preconditions failed, optimistic concurrency check failed
                    if (ex.Status == 412)
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
                        await leaseClient.ReleaseAsync(new RequestConditions()).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // TODO: investigate what can happen here
                }
            }

            return SnapshotBlobStatus.OK;
        }

        public async Task<Uri> GetSharedAccessUriAsync(string blobName, string directoryRelativeAddress = null, BlobSasPermissions permissions = BlobSasPermissions.Read, DateTimeOffset? startTime = null, DateTimeOffset? expiryTime = null, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(blobName), blobName);

            directoryRelativeAddress ??= _defaultDirectoryRelativeAddress;
            startTime ??= DateTimeOffset.UtcNow.AddMinutes(-5);
            expiryTime ??= DateTimeOffset.UtcNow.Add(_defaultSharedAccessDuration);
            var blobClient = GetBlobClient(blobName, directoryRelativeAddress);

            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = startTime.Value,
                ExpiresOn = expiryTime.Value,
                Protocol = SasProtocol.Https,
            };

            sasBuilder.SetPermissions(permissions);

            if (blobClient.CanGenerateSasUri)
            {
                return CleanUpUri(blobClient.GenerateSasUri(sasBuilder));
            }

            if (_storageSharedKeyCredential != null)
            {
                return CleanUpUri(new BlobUriBuilder(blobClient.Uri)
                {
                    Sas = sasBuilder.ToSasQueryParameters(_storageSharedKeyCredential),
                }.ToUri());
            }

            if (_useManagedIdentity)
            {
                var userDelegationKey = (await _accountClient.GetUserDelegationKeyAsync(null, expiryTime.Value, cancellationToken).ConfigureAwait(false)).Value;

                return CleanUpUri(new BlobUriBuilder(blobClient.Uri)
                {
                    Sas = sasBuilder.ToSasQueryParameters(userDelegationKey, blobClient.AccountName),
                }.ToUri());
            }

            throw new InvalidOperationException("Must use Shared Account Key or Token Credentials (including Managed Identity) assigned the Storage Blob Delegator role to generate a Shared Access Signature (SAS).");
        }

        public async Task CreateIfNotExistsAsync(PublicAccessType publicAccessType, CancellationToken cancellationToken = default)
        {
            await _containerClient.CreateIfNotExistsAsync(publicAccessType, null, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveExpiredAsync(string directoryRelativeAddress = null, CancellationToken cancellationToken = default)
        {
            directoryRelativeAddress ??= _defaultDirectoryRelativeAddress;
            var utcNow = DateTimeOffset.UtcNow;

            await foreach (var blob in _containerClient.GetBlobsAsync(BlobTraits.Metadata, BlobStates.None, directoryRelativeAddress, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // skip non-expired
                if (!blob.Properties.ExpiresOn.HasValue || blob.Properties.ExpiresOn.Value > utcNow)
                {
                    continue;
                }

                try
                {
                    await _containerClient.DeleteBlobIfExistsAsync(blob.Name, DeleteSnapshotsOption.IncludeSnapshots, new BlobRequestConditions() { IfMatch = blob.Properties.ETag }, cancellationToken).ConfigureAwait(false);
                }
                catch (RequestFailedException ex)
                {
                    // 404: not found
                    if (ex.Status == 404)
                    {
                        continue;
                    }

                    // 412: preconditions failed, optimistic concurrency check failed
                    if (ex.Status == 412)
                    {
                        continue;
                    }

                    throw;
                }
            }
        }

        private BlobClient GetBlobClient(string blobName, string directoryRelativeAddress)
        {
            if (!string.IsNullOrEmpty(directoryRelativeAddress))
            {
                blobName = $"{directoryRelativeAddress.TrimEnd('/')}/{blobName}";
            }

            return _containerClient.GetBlobClient(blobName);
        }

        async Task<bool> IDataStore.ExistsAsync(string name, string partition, CancellationToken cancellationToken)
        {
            return await ExistsAsync(name, partition, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IEnumerable<IBlobMeta>> IDataStore.ListAsync(string partition, CancellationToken cancellationToken)
        {
            return await ListAsync(partition, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IBlobResult> IDataStore.GetAsync(string name, string partition, object concurrencyToken, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            return await GetStreamAsync(name, partition, (string)concurrencyToken, throwOnNotFound: throwOnNotFound, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<object> IDataStore.AddOrReplaceAsync(BlobData blob, CancellationToken cancellationToken)
        {
            ArgCheck.NotNull(nameof(blob), blob);

            return (await PutAsync(blob?.Key, blob?.Content, directoryRelativeAddress: blob?.Partition, contentType: blob?.ContentType, contentEncoding: blob?.ContentEncoding, ifMatchETag: (string)blob?.ConcurrencyToken, absoluteExpiration: blob?.ExpiresAt, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task IDataStore.RemoveAsync(string key, string partition, CancellationToken cancellationToken)
        {
            await DeleteAsync(key, partition, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<Uri> ISharedUriAccessible.GetSharedUriAsync(string name, string partition, AccessPermissions permissions, DateTimeOffset? startTime, DateTimeOffset? expiryTime, CancellationToken cancellationToken)
        {
            BlobSasPermissions sasPermissions = default;

            if (permissions.HasFlagSet(AccessPermissions.Read))
            {
                sasPermissions |= BlobSasPermissions.Read;
            }

            if (permissions.HasFlagSet(AccessPermissions.Write))
            {
                sasPermissions |= BlobSasPermissions.Write;
            }

            if (permissions.HasFlagSet(AccessPermissions.Delete))
            {
                sasPermissions |= BlobSasPermissions.Delete;
            }

            return await GetSharedAccessUriAsync(name, partition, sasPermissions, startTime, expiryTime, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IStringResult> IKeyValueReadOnlyStore.GetStringAsync(string key, string partition, object concurrencyToken, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            return await GetStringAsync(key, partition, (string)concurrencyToken, throwOnNotFound: throwOnNotFound, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IBytesResult> IKeyValueReadOnlyStore.GetBytesAsync(string key, string partition, object concurrencyToken, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            return await GetBytesAsync(key, partition, (string)concurrencyToken, throwOnNotFound: throwOnNotFound, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<IStreamResult> IKeyValueReadOnlyStore.GetStreamAsync(string key, string partition, object concurrencyToken, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            return await GetStreamAsync(key, partition, (string)concurrencyToken, throwOnNotFound: throwOnNotFound, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        async Task<object> IKeyValueStore.AddOrReplaceAsync(string key, string value, string partition, object concurrencyToken, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken)
        {
            return (await PutAsync(key, value, directoryRelativeAddress: partition, ifMatchETag: (string)concurrencyToken, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task<object> IKeyValueStore.AddOrReplaceAsync(string key, byte[] bytes, string partition, object concurrencyToken, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken)
        {
            return (await PutAsync(key, bytes, directoryRelativeAddress: partition, ifMatchETag: (string)concurrencyToken, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
        }

        async Task<object> IKeyValueStore.AddOrReplaceAsync(string key, Stream stream, string partition, object concurrencyToken, DateTimeOffset? absoluteExpiration, CancellationToken cancellationToken)
        {
            return (await PutAsync(key, stream, directoryRelativeAddress: partition, ifMatchETag: (string)concurrencyToken, cancellationToken: cancellationToken).ConfigureAwait(false))?.ETag;
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

        private static Uri CleanUpUri(Uri uri)
        {
            var b = new UriBuilder(uri);
            b.Path = b.Path.Replace("%2f", "/");
            return b.Uri;
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace KodeAid.Azure.Storage
{
    public static class AzureBlobStorageKeyValueStoreExtensions
    {
        internal static AzureBlobStorageKeyValueStoreOptions Verify(this AzureBlobStorageKeyValueStoreOptions options)
        {
            if (options?.StorageAccount == null)
            {
                if (!string.IsNullOrEmpty(options?.ConnectionString))
                {
                    options.StorageAccount = CloudStorageAccount.Parse(options.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(options?.AccountName) && !string.IsNullOrEmpty(options?.SharedAccessSignature))
                {
                    options.StorageAccount = new CloudStorageAccount(new StorageCredentials(options.SharedAccessSignature), options.AccountName, options.EndpointSuffix ?? "core.windows.net", true);
                }
                else if (options.SecretStore != null && (!string.IsNullOrEmpty(options?.ConnectionStringSecretName) || !string.IsNullOrEmpty(options?.SharedAccessSignatureSecretName)))
                {
                    //options.StorageAccount = new CloudStorageAccount(new StorageCredentials(options.SharedAccessSignature), options.AccountName, options.EndpointSuffix ?? "core.windows.net", true);
                }
                else
                {
                    ArgCheck.NotNull(nameof(options.StorageAccount), options?.StorageAccount);
                }
            }

            ArgCheck.NotNullOrEmpty(nameof(options.ContainerName), options?.ContainerName);

            if (options.LeaseDuration.HasValue)
            {
                // as per Azure storage lease duration requirements
                ArgCheck.GreaterThanOrEqualTo(nameof(options.LeaseDuration), options.LeaseDuration, TimeSpan.FromSeconds(15));
                ArgCheck.LessThanOrEqualTo(nameof(options.LeaseDuration), options.LeaseDuration, TimeSpan.FromSeconds(60));
            }

            return options;
        }
    }
}

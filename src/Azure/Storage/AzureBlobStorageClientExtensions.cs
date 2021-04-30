// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Azure.Storage
{
    public static class AzureBlobStorageClientExtensions
    {
        internal static AzureBlobStorageClientOptions Verify(this AzureBlobStorageClientOptions options)
        {

            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                if (string.IsNullOrEmpty(options.AccountName))
                {
                    throw new ArgumentException("Connection string or account name is required.", nameof(options));
                }

                if (string.IsNullOrEmpty(options.AccountKey) && string.IsNullOrEmpty(options.SharedAccessSignature) && !options.UseManagedIdentity)
                {
                    throw new ArgumentException("Account key, SAS or managed identity is required.", nameof(options));
                }
            }

            if (options.LeaseDuration.HasValue)
            {
                // As per Azure storage lease duration constraints.
                ArgCheck.GreaterThanOrEqualTo(nameof(options.LeaseDuration), options.LeaseDuration, TimeSpan.FromSeconds(15));
                ArgCheck.LessThanOrEqualTo(nameof(options.LeaseDuration), options.LeaseDuration, TimeSpan.FromSeconds(60));
            }

            return options;
        }
    }
}

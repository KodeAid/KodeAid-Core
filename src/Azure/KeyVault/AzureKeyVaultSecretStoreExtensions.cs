// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.Azure.KeyVault
{
    public static class AzureKeyVaultSecretStoreExtensions
    {
        internal static AzureKeyVaultSecretStoreOptions Verify(this AzureKeyVaultSecretStoreOptions options)
        {
            if (options.KeyVaultBaseUrl == null && options.KeyVaultName != null)
            {
                options.KeyVaultBaseUrl = $"https://{options.KeyVaultName.ToLower()}.{(options.EndpointSuffix ?? "vault.azure.net")}";
            }

            ArgCheck.NotNullOrEmpty(nameof(options.KeyVaultBaseUrl), options?.KeyVaultBaseUrl);

            return options;
        }
    }
}

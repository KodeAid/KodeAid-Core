// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.Azure.KeyVault
{
    public sealed class AzureKeyVaultSecretStoreOptions
    {
        /// <summary>
        /// Base URL of the Azure key vault holding the secrets, eg. "https://mykeyvault.vault.azure.net"
        /// </summary>
        public string KeyVaultBaseUrl { get; set; }
    }
}

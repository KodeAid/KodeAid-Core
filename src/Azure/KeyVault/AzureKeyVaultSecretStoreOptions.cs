// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Security.Cryptography.X509Certificates;

namespace KodeAid.Azure.KeyVault
{
    public sealed class AzureKeyVaultSecretStoreOptions
    {
        /// <summary>
        /// Name of the key vault, eg. "MyKeyvault"
        /// </summary>
        public string KeyVaultName { get; set; }

        /// <summary>
        /// Eg. "vault.azure.net"
        /// </summary>
        public string EndpointSuffix { get; set; }

        /// <summary>
        /// Base URL of the Azure key vault holding the secrets, eg. "https://mykeyvault.vault.azure.net"
        /// </summary>
        public string KeyVaultBaseUrl { get; set; }

        /// <summary>
        /// Indicates how to store private keys imported from Azure key vault secrets.
        /// </summary>
        public X509KeyStorageFlags KeyStorageFlags { get; set; } = X509KeyStorageFlags.MachineKeySet;
    }
}

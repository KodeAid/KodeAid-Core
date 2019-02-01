// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Security.Cryptography.X509Certificates;

namespace KodeAid.Azure.KeyVault
{
    public sealed class AzureKeyVaultSecretStoreOptionsBuilder
    {
        private string _keyVaultName;
        private string _endpointSuffix;
        private string _keyVaultBaseUrl;
        private X509KeyStorageFlags _keyStorageFlags = X509KeyStorageFlags.MachineKeySet;

        public AzureKeyVaultSecretStoreOptions Build()
        {
            return new AzureKeyVaultSecretStoreOptions()
            {
                KeyVaultBaseUrl = _keyVaultBaseUrl,
                KeyVaultName = _keyVaultName,
                EndpointSuffix = _endpointSuffix,
                KeyStorageFlags = _keyStorageFlags,
            };
        }

        /// <summary>
        /// Sets the name of the Azure key vault (eg. "MyKeyVault") and optionally the endpoint suffix which by default is ("vault.azure.net").
        /// </summary>
        public AzureKeyVaultSecretStoreOptionsBuilder WithKeyVault(string keyVaultName, string endpointSuffix = null)
        {
            _keyVaultName = keyVaultName;
            _endpointSuffix = endpointSuffix;
            return this;
        }

        /// <summary>
        /// Sets the base URL of the Azure key vault, eg. "https://mykeyvault.vault.azure.net".
        /// </summary>
        public AzureKeyVaultSecretStoreOptionsBuilder WithKeyVaultBaseUrl(string keyVaultBaseUrl)
        {
            _keyVaultBaseUrl = keyVaultBaseUrl;
            return this;
        }

        /// <summary>
        /// Indicates how to store private keys imported from Azure key vault secrets.
        /// </summary>
        public AzureKeyVaultSecretStoreOptionsBuilder WithKeyStorageFlags(X509KeyStorageFlags keyStorageFlags)
        {
            _keyStorageFlags = keyStorageFlags;
            return this;
        }
    }
}

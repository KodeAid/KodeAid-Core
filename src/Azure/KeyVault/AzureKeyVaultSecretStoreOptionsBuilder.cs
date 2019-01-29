// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.Azure.KeyVault
{
    public sealed class AzureKeyVaultSecretStoreOptionsBuilder
    {
        private string _keyVaultBaseUrl;

        public AzureKeyVaultSecretStoreOptions Build()
        {
            return new AzureKeyVaultSecretStoreOptions()
            {
                KeyVaultBaseUrl = _keyVaultBaseUrl,
            }
            .Verify();
        }

        /// <summary>
        /// Sets the base URL of the Azure key vault holding the secrets, eg. "https://mykeyvault.vault.azure.net"
        /// </summary>
        public AzureKeyVaultSecretStoreOptionsBuilder WithKeyVaultBaseUrl(string keyVaultBaseUrl)
        {
            ArgCheck.NotNullOrEmpty(nameof(keyVaultBaseUrl), keyVaultBaseUrl);

            _keyVaultBaseUrl = keyVaultBaseUrl;

            return this;
        }
    }
}

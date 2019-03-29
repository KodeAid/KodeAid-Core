// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using KodeAid.Security.Cryptography.X509Certificates;
using KodeAid.Security.Secrets;
using Microsoft.Azure.KeyVault;

namespace KodeAid.Azure.KeyVault
{
    public class AzureKeyVaultSecretStore : ISecretReadOnlyStore, IPrivateCertificateStore
    {
        private readonly string _keyVaultBaseUrl;
        private readonly X509KeyStorageFlags _keyStorageFlags = X509KeyStorageFlags.MachineKeySet;

        public AzureKeyVaultSecretStore(AzureKeyVaultSecretStoreOptions options)
        {
            ArgCheck.NotNull(nameof(options), options);
            options.Verify();

            _keyVaultBaseUrl = options.KeyVaultBaseUrl?.TrimEnd(' ', '/');
            _keyStorageFlags = options.KeyStorageFlags;
        }

        public async Task<SecureString> GetSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(name), name);

            using (var client = new ManagedServiceIdentityKeyVaultClient())
            {
                var secretBundle = await client.GetSecretAsync(_keyVaultBaseUrl, name).ConfigureAwait(false);

                var secureString = new SecureString();
                foreach (var c in secretBundle.Value)
                {
                    secureString.AppendChar(c);
                }
                secureString.MakeReadOnly();

                return secureString;
            }
        }

        public async Task<X509Certificate2> GetPrivateCertificateAsync(string name, CancellationToken cancellationToken = default)
        {
            var securedBase64Key = await GetSecretAsync(name, cancellationToken).ConfigureAwait(false);
            return new X509Certificate2(Convert.FromBase64String(securedBase64Key.Unsecure()), (string)null, _keyStorageFlags);
        }
    }
}

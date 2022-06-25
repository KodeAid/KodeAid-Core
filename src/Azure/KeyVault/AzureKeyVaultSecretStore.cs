// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KodeAid.Security.Cryptography.X509Certificates;
using KodeAid.Security.Secrets;

namespace KodeAid.Azure.KeyVault
{
    public class AzureKeyVaultSecretStore : ISecretReadOnlyStore, IPrivateCertificateStore
    {
        private readonly SecretClient _client;
        private readonly X509KeyStorageFlags _keyStorageFlags = X509KeyStorageFlags.MachineKeySet;

        public AzureKeyVaultSecretStore(AzureKeyVaultSecretStoreOptions options)
        {
            ArgCheck.NotNull(nameof(options), options);
            options.Verify();

            var keyVaultBaseUrl = options.KeyVaultBaseUrl?.TrimEnd(' ', '/');
            _keyStorageFlags = options.KeyStorageFlags;
            _client = new SecretClient(new Uri(keyVaultBaseUrl), new DefaultAzureCredential());
        }

        public async Task<SecureString> GetSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(name), name);

            var unsecuredSecret = await GetUnsecuredSecretAsync(name, cancellationToken).ConfigureAwait(false);

            var securedSecret = new SecureString();
            foreach (var c in unsecuredSecret)
            {
                securedSecret.AppendChar(c);
            }
            securedSecret.MakeReadOnly();

            return securedSecret;
        }

        public async Task<X509Certificate2> GetPrivateCertificateAsync(string name, CancellationToken cancellationToken = default)
        {
            var keyBase64String = await GetUnsecuredSecretAsync(name, cancellationToken).ConfigureAwait(false);
            var keyBytes = keyBase64String.FromBase64String();
            return new X509Certificate2(keyBytes, (string)null, _keyStorageFlags);
        }

        private async Task<string> GetUnsecuredSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(name), name);

            return (await _client.GetSecretAsync(name).ConfigureAwait(false)).Value.Value;
        }
    }
}

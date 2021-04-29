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
    public class AzureKeyVaultSecretStore : ISecretReadOnlyStore, IPrivateCertificateStore, IDisposable
    {
        private readonly string _keyVaultBaseUrl;
        private readonly X509KeyStorageFlags _keyStorageFlags = X509KeyStorageFlags.MachineKeySet;
        private readonly ManagedServiceIdentityKeyVaultClient _client = new();
        private bool _disposed = false;

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

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                _disposed = true;
            }
        }

        private async Task<string> GetUnsecuredSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(name), name);

            return (await _client.GetSecretAsync(_keyVaultBaseUrl, name, cancellationToken).ConfigureAwait(false)).Value;
        }
    }
}

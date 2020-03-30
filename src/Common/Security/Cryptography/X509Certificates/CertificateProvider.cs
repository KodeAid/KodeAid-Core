// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Security.Cryptography.X509Certificates
{
    public class CertificateProvider : ICertificateProvider
    {
        private readonly IPrivateCertificateStore _privateKeyStore;
        private readonly IPublicCertificateStore _publicKeyStore;

        public CertificateProvider(IPrivateCertificateStore privateKeyStore, IPublicCertificateStore publicKeyStore)
        {
            ArgCheck.NotNull(nameof(privateKeyStore), privateKeyStore);
            ArgCheck.NotNull(nameof(publicKeyStore), publicKeyStore);

            _privateKeyStore = privateKeyStore;
            _publicKeyStore = publicKeyStore;
        }

        public Task<X509Certificate2> GetCertificateAsync(string name, bool withPrivateKey = false, CancellationToken cancellationToken = default)
        {
            ArgCheck.NotNullOrEmpty(nameof(name), name);

            if (withPrivateKey)
            {
                return _privateKeyStore.GetPrivateCertificateAsync(name, cancellationToken);
            }
            else
            {
                return _publicKeyStore.GetPublicCertificateAsync(name, cancellationToken);
            }
        }
    }
}

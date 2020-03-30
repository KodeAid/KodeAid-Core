// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Security.Cryptography.X509Certificates
{
    public interface ICertificateProvider
    {
        Task<X509Certificate2> GetCertificateAsync(string name, bool withPrivateKey = false, CancellationToken cancellationToken = default);
    }
}

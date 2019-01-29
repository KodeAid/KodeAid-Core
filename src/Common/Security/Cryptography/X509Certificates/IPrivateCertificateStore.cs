// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Security.Cryptography.X509Certificates
{
    public interface IPrivateCertificateStore
    {
        Task<X509Certificate2> GetPrivateCertificateAsync(string name, CancellationToken cancellationToken = default);
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace KodeAid.Security.Cryptography.X509Certificates
{
    public static class CertificateHelper
    {
        public static X509Certificate2 GetCertificate(CertificateSource source, string value)
        {
            ArgCheck.NotNullOrDefault(nameof(source), source);
            ArgCheck.NotNullOrEmpty(nameof(value), value);

            if (source == CertificateSource.FileName)
            {
                var fileName = value;
                if (!File.Exists(fileName))
                    throw new ArgumentException($"Certificate at {fileName} not found.", nameof(fileName));
                return new X509Certificate2(fileName);
            }

            if (source == CertificateSource.Thumbprint)
            {
                var thumbprint = value;
                using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadOnly);
                    var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                    if (certs.Count == 0)
                        throw new ArgumentException($"Certificate with thumbprint {thumbprint} not found.", nameof(thumbprint));
                    return certs[0];
                }
            }

            throw new ArgumentOutOfRangeException(nameof(source), $"Parameter {nameof(source)} is not a valid value.");
        }
    }
}

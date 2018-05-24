// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using KodeAid.Security.Cryptography.X509Certificates;

namespace KodeAid.Security.Cryptography
{
    public class RsaCertificateProtector : IDataProtector
    {
        private readonly X509Certificate2 _certificate;
        private readonly RSAEncryptionPadding _padding;

        public RsaCertificateProtector(X509Certificate2 certificate)
            : this(certificate, null)
        {
        }

        public RsaCertificateProtector(X509Certificate2 certificate, RSAEncryptionPadding padding)
        {
            ArgCheck.NotNull(nameof(certificate), certificate);
            _certificate = certificate;
            _padding = padding ?? RSAEncryptionPadding.OaepSHA1;
        }

        public byte[] ProtectData(byte[] unprotectedData)
        {
            ArgCheck.NotNull(nameof(unprotectedData), unprotectedData);
            return _certificate.GetRSAPrivateKey().Encrypt(unprotectedData, _padding);
        }

        public byte[] UnprotectData(byte[] protectedData)
        {
            ArgCheck.NotNull(nameof(protectedData), protectedData);
            return _certificate.GetRSAPrivateKey().Decrypt(protectedData, _padding);
        }
    }
}

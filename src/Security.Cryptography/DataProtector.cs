// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using KodeAid.Security.Cryptography.X509Certificates;

namespace KodeAid.Security.Cryptography
{
    public static class DataProtector
    {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;

        public static byte[] EncryptStringWithPassword(string password, string value)
        {
            return EncryptStringWithPassword(password, _defaultEncoding, value);
        }

        public static byte[] EncryptStringWithMachineKey(string value)
        {
            return EncryptStringWithMachineKey(_defaultEncoding, value);
        }

        public static byte[] EncryptStringWithCertificate(CertificateSource certificateSource, string certificateData, string value)
        {
            return EncryptStringWithCertificate(certificateSource, certificateData, _defaultEncoding, value);
        }

        public static byte[] EncryptStringWithCertificate(X509Certificate2 certificate, string value)
        {
            return EncryptStringWithCertificate(certificate, _defaultEncoding, value);
        }

        public static byte[] EncryptStringsWithPassword(string password, params string[] values)
        {
            return EncryptStringsWithPassword(password, _defaultEncoding, values);
        }

        public static byte[] EncryptStringsWithMachineKey(params string[] values)
        {
            return EncryptStringsWithMachineKey(_defaultEncoding, values);
        }

        public static byte[] EncryptStringsWithCertificate(CertificateSource certificateSource, string certificateData, params string[] values)
        {
            return EncryptStringsWithCertificate(certificateSource, certificateData, _defaultEncoding, values);
        }

        public static byte[] EncryptStringsWithCertificate(X509Certificate2 certificate, params string[] values)
        {
            return EncryptStringsWithCertificate(certificate, _defaultEncoding, values);
        }

        public static byte[] EncryptStringWithPassword(string password, Encoding encoding, string value)
        {
            return EncryptStringsWithPassword(password, encoding, new[] { value });
        }

        public static byte[] EncryptStringWithMachineKey(Encoding encoding, string value)
        {
            return EncryptStringsWithMachineKey(encoding, new[] { value });
        }

        public static byte[] EncryptStringWithCertificate(CertificateSource certificateSource, string certificateData, Encoding encoding, string value)
        {
            return EncryptStringsWithCertificate(certificateSource, certificateData, encoding, new[] { value });
        }

        public static byte[] EncryptStringWithCertificate(X509Certificate2 certificate, Encoding encoding, string value)
        {
            return EncryptStringsWithCertificate(certificate, encoding, new[] { value });
        }

        public static byte[] EncryptStringsWithPassword(string password, Encoding encoding, params string[] values)
        {
            return EncryptBytesWithPassword(password, GetBytesFromStrings(encoding, values));
        }

        public static byte[] EncryptStringsWithMachineKey(Encoding encoding, params string[] values)
        {
            return EncryptBytesWithMachineKey(GetBytesFromStrings(encoding, values));
        }

        public static byte[] EncryptStringsWithCertificate(CertificateSource certificateSource, string certificateData, Encoding encoding, params string[] values)
        {
            return EncryptBytesWithCertificate(certificateSource, certificateData, GetBytesFromStrings(encoding, values));
        }

        public static byte[] EncryptStringsWithCertificate(X509Certificate2 certificate, Encoding encoding, params string[] values)
        {
            return EncryptBytesWithCertificate(certificate, GetBytesFromStrings(encoding, values));
        }

        public static byte[] EncryptBytesWithPassword(string password, byte[] data)
        {
            ArgCheck.NotNullOrEmpty(nameof(password), password);
            ArgCheck.NotNullOrEmpty(nameof(data), data);
            return new SimpleAES().EncryptData(data, _defaultEncoding.GetBytes(password));
        }

        public static byte[] EncryptBytesWithMachineKey(byte[] data)
        {
            ArgCheck.NotNullOrEmpty(nameof(data), data);
            return ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);
        }

        public static byte[] EncryptBytesWithCertificate(CertificateSource certificateSource, string certificateData, byte[] data)
        {
            return EncryptBytesWithCertificate(CertificateHelper.GetCertificate(certificateSource, certificateData), data);
        }

        public static byte[] EncryptBytesWithCertificate(X509Certificate2 certificate, byte[] data)
        {
            ArgCheck.NotNull(nameof(certificate), certificate);
            ArgCheck.NotNullOrEmpty(nameof(data), data);
            return certificate.GetRSAPrivateKey().Encrypt(data, RSAEncryptionPadding.OaepSHA1);
        }

        public static string DecryptStringWithPassword(string password, byte[] encryptedData)
        {
            return DecryptStringWithPassword(password, _defaultEncoding, encryptedData);
        }

        public static string DecryptStringWithMachineKey(byte[] encryptedData)
        {
            return DecryptStringWithMachineKey(_defaultEncoding, encryptedData);
        }

        public static string DecryptStringWithCertificate(CertificateSource certificateSource, string certificateData, byte[] encryptedData)
        {
            return DecryptStringWithCertificate(certificateSource, certificateData, _defaultEncoding, encryptedData);
        }

        public static string DecryptStringWithCertificate(X509Certificate2 certificate, byte[] encryptedData)
        {
            return DecryptStringWithCertificate(certificate, _defaultEncoding, encryptedData);
        }

        public static string[] DecryptStringsWithPassword(string password, byte[] encryptedData)
        {
            return DecryptStringsWithPassword(password, _defaultEncoding, encryptedData);
        }

        public static string[] DecryptStringsWithMachineKey(byte[] encryptedData)
        {
            return DecryptStringsWithMachineKey(_defaultEncoding, encryptedData);
        }

        public static string[] DecryptStringsWithCertificate(CertificateSource certificateSource, string certificateData, byte[] encryptedData)
        {
            return DecryptStringsWithCertificate(certificateSource, certificateData, _defaultEncoding, encryptedData);
        }

        public static string[] DecryptStringsWithCertificate(X509Certificate2 certificate, byte[] encryptedData)
        {
            return DecryptStringsWithCertificate(certificate, _defaultEncoding, encryptedData);
        }

        public static string DecryptStringWithPassword(string password, Encoding encoding, byte[] encryptedData)
        {
            return DecryptStringsWithPassword(password, encoding, encryptedData)[0];
        }

        public static string DecryptStringWithMachineKey(Encoding encoding, byte[] encryptedData)
        {
            return DecryptStringsWithMachineKey(encoding, encryptedData)[0];
        }

        public static string DecryptStringWithCertificate(CertificateSource certificateSource, string certificateData, Encoding encoding, byte[] encryptedData)
        {
            return DecryptStringsWithCertificate(certificateSource, certificateData, encoding, encryptedData)[0];
        }

        public static string DecryptStringWithCertificate(X509Certificate2 certificate, Encoding encoding, byte[] encryptedData)
        {
            return DecryptStringsWithCertificate(certificate, encoding, encryptedData)[0];
        }

        public static string[] DecryptStringsWithPassword(string password, Encoding encoding, byte[] encryptedData)
        {
            return GetStringsFromBytes(encoding, DecryptBytesWithPassword(password, encryptedData));
        }

        public static string[] DecryptStringsWithMachineKey(Encoding encoding, byte[] encryptedData)
        {
            return GetStringsFromBytes(encoding, DecryptBytesWithMachineKey(encryptedData));
        }

        public static string[] DecryptStringsWithCertificate(CertificateSource certificateSource, string certificateData, Encoding encoding, byte[] encryptedData)
        {
            return GetStringsFromBytes(encoding, DecryptBytesWithCertificate(certificateSource, certificateData, encryptedData));
        }

        public static string[] DecryptStringsWithCertificate(X509Certificate2 certificate, Encoding encoding, byte[] encryptedData)
        {
            return GetStringsFromBytes(encoding, DecryptBytesWithCertificate(certificate, encryptedData));
        }

        public static byte[] DecryptBytesWithPassword(string password, byte[] encryptedData)
        {
            ArgCheck.NotNullOrEmpty(nameof(encryptedData), encryptedData);
            ArgCheck.NotNullOrEmpty(nameof(password), password);
            return new SimpleAES().DecryptData(encryptedData, _defaultEncoding.GetBytes(password));
        }

        public static byte[] DecryptBytesWithMachineKey(byte[] encryptedData)
        {
            ArgCheck.NotNullOrEmpty(nameof(encryptedData), encryptedData);
            return ProtectedData.Protect(encryptedData, null, DataProtectionScope.LocalMachine);
        }

        public static byte[] DecryptBytesWithCertificate(CertificateSource certificateSource, string certificateData, byte[] encryptedData)
        {
            return DecryptBytesWithCertificate(CertificateHelper.GetCertificate(certificateSource, certificateData), encryptedData);
        }

        public static byte[] DecryptBytesWithCertificate(X509Certificate2 certificate, byte[] encryptedData)
        {
            ArgCheck.NotNullOrEmpty(nameof(encryptedData), encryptedData);
            ArgCheck.NotNull(nameof(certificate), certificate);
            return certificate.GetRSAPrivateKey().Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA1);
        }

        private static byte[] GetBytesFromStrings(Encoding encoding, string[] values)
        {
            ArgCheck.NotNull(nameof(encoding), encoding);
            ArgCheck.NotNullOrEmpty(nameof(values), values);

            using (var ms = new MemoryStream())
            {
                using (var w = new BinaryWriter(ms, encoding, true))
                {
                    w.Write(values.Length);
                    foreach (var v in values)
                    {
                        w.Write(v != null);
                        if (v != null)
                            w.Write(v);
                    }
                    w.Flush();
                }
                ms.Flush();
                return ms.ToArray();
            }
        }

        private static string[] GetStringsFromBytes(Encoding encoding, byte[] data)
        {
            ArgCheck.NotNull(nameof(encoding), encoding);
            ArgCheck.NotNullOrEmpty(nameof(data), data);

            var values = new List<string>();
            using (var ms = new MemoryStream(data))
            {
                using (var r = new BinaryReader(ms, encoding, true))
                {
                    for (var i = r.ReadInt32(); i > 0; --i)
                    {
                        if (r.ReadBoolean())
                            values.Add(r.ReadString());
                        else
                            values.Add(null);
                    }
                }
            }
            return values.ToArray();
        }
    }
}

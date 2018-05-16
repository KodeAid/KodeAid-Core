// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace KodeAid.Security.Cryptography
{
    public class SimpleAES : IDataProtector
    {
        public byte[] EncryptData(byte[] dataToEncrypt, byte[] key)
        {
            ArgCheck.NotNullOrEmpty("dataToEncrypt", dataToEncrypt);
            ArgCheck.NotNullOrEmpty("key", key);
            var iv = Guid.NewGuid().ToByteArray();
            using (var rm = new RijndaelManaged())
                return Transform(dataToEncrypt, rm.CreateEncryptor(GetValidKey(key), iv)).Concat(iv).ToArray();
        }

        public byte[] DecryptData(byte[] dataToDecrypt, byte[] key)
        {
            ArgCheck.NotNullOrEmpty("dataToDecrypt", dataToDecrypt);
            ArgCheck.NotNullOrEmpty("key", key);
            if (dataToDecrypt.Length < 16)
                throw new ArgumentException("Parameter dataToDecrypt must be at least 16 bytes long.", "dataToDecrypt");
            var iv = dataToDecrypt.Skip(dataToDecrypt.Length - 16).ToArray();
            dataToDecrypt = dataToDecrypt.Take(dataToDecrypt.Length - 16).ToArray();
            using (var rm = new RijndaelManaged())
                return Transform(dataToDecrypt, rm.CreateDecryptor(GetValidKey(key), iv));
        }

        private byte[] GetValidKey(byte[] suggestedKey)
        {
            ArgCheck.NotNullOrEmpty("suggestedKey", suggestedKey);
            while (suggestedKey.Length < 32)
                suggestedKey = suggestedKey.Concat(suggestedKey).ToArray();
            if (suggestedKey.Length > 32)
                suggestedKey = suggestedKey.Take(32).ToArray();
            return suggestedKey;
        }

        private byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                    cs.Write(buffer, 0, buffer.Length);
                return ms.ToArray();
            }
        }
    }
}

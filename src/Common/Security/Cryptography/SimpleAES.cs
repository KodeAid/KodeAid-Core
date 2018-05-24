// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KodeAid.Security.Cryptography
{
    public class SimpleAES : IDataProtector
    {
        private readonly byte[] _key;

        public SimpleAES(byte[] key)
        {
            ArgCheck.NotNull(nameof(key), key);
            _key = GetValidKey(key);
        }

        public byte[] ProtectData(byte[] unprotectedData)
        {
            ArgCheck.NotNull(nameof(unprotectedData), unprotectedData);
            var iv = Guid.NewGuid().ToByteArray();
            using (var rm = new RijndaelManaged())
                return Transform(unprotectedData, rm.CreateEncryptor(_key, iv)).Concat(iv).ToArray();
        }

        public async Task<byte[]> ProtectDataAsync(byte[] unprotectedData)
        {
            ArgCheck.NotNull(nameof(unprotectedData), unprotectedData);
            var iv = Guid.NewGuid().ToByteArray();
            using (var rm = new RijndaelManaged())
                return (await TransformAsync(unprotectedData, rm.CreateEncryptor(_key, iv)).ConfigureAwait(false)).Concat(iv).ToArray();
        }

        public byte[] UnprotectData(byte[] protectedData)
        {
            ArgCheck.NotNull(nameof(protectedData), protectedData);
            if (protectedData.Length < 16)
                throw new ArgumentException($"Parameter {nameof(protectedData)} must be at least 16 bytes long.", nameof(protectedData));
            var iv = protectedData.Skip(protectedData.Length - 16).ToArray();
            protectedData = protectedData.Take(protectedData.Length - 16).ToArray();
            using (var rm = new RijndaelManaged())
                return Transform(protectedData, rm.CreateDecryptor(_key, iv));
        }

        public async Task<byte[]> UnprotectDataAsync(byte[] protectedData)
        {
            ArgCheck.NotNull(nameof(protectedData), protectedData);
            if (protectedData.Length < 16)
                throw new ArgumentException($"Parameter {nameof(protectedData)} must be at least 16 bytes long.", nameof(protectedData));
            var iv = protectedData.Skip(protectedData.Length - 16).ToArray();
            protectedData = protectedData.Take(protectedData.Length - 16).ToArray();
            using (var rm = new RijndaelManaged())
                return await TransformAsync(protectedData, rm.CreateDecryptor(_key, iv)).ConfigureAwait(false);
        }

        private byte[] GetValidKey(byte[] suggestedKey)
        {
            ArgCheck.NotNullOrEmpty(nameof(suggestedKey), suggestedKey);
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
                {
                    cs.Write(buffer, 0, buffer.Length);
                    cs.Flush();
                }
                ms.Flush();
                return ms.ToArray();
            }
        }

        private async Task<byte[]> TransformAsync(byte[] buffer, ICryptoTransform transform)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    await cs.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    await cs.FlushAsync().ConfigureAwait(false);
                }
                await ms.FlushAsync().ConfigureAwait(false);
                return ms.ToArray();
            }
        }
    }
}

// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Text;

namespace KodeAid.Security.Cryptography
{
    public static class DataProtectorExtensions
    {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;

        public static string EncryptStringToBase64(this IDataProtector dataProtector, string textToEncrypt, string password, bool urlEncoded = false, Encoding encoding = null)
        {
            ArgCheck.NotNull(nameof(dataProtector), dataProtector);
            ArgCheck.NotNullOrEmpty(nameof(textToEncrypt), textToEncrypt);
            ArgCheck.NotNullOrEmpty(nameof(password), password);
            return EncryptDataToBase64(dataProtector, (encoding ?? _defaultEncoding).GetBytes(textToEncrypt), password, urlEncoded, encoding);
        }

        public static string DecryptStringFromBase64(this IDataProtector dataProtector, string base64ToDecrypt, string password, bool urlEncoded = false, Encoding encoding = null)
        {
            ArgCheck.NotNull(nameof(dataProtector), dataProtector);
            ArgCheck.NotNullOrEmpty(nameof(base64ToDecrypt), base64ToDecrypt);
            ArgCheck.NotNullOrEmpty(nameof(password), password);
            return (encoding ?? _defaultEncoding).GetString(DecryptDataFromBase64(dataProtector, base64ToDecrypt, password, urlEncoded, encoding));
        }

        public static string EncryptDataToBase64(this IDataProtector dataProtector, byte[] dataToEncrypt, string password, bool urlEncoded = false, Encoding encoding = null)
        {
            ArgCheck.NotNull(nameof(dataProtector), dataProtector);
            ArgCheck.NotNullOrEmpty(nameof(dataToEncrypt), dataToEncrypt);
            ArgCheck.NotNullOrEmpty(nameof(password), password);
            return dataProtector.EncryptData(dataToEncrypt, (encoding ?? _defaultEncoding).GetBytes(password)).ToBase64(urlEncoded);
        }

        public static byte[] DecryptDataFromBase64(this IDataProtector dataProtector, string base64ToDecrypt, string password, bool urlEncoded = false, Encoding encoding = null)
        {
            ArgCheck.NotNull(nameof(dataProtector), dataProtector);
            ArgCheck.NotNullOrEmpty(nameof(base64ToDecrypt), base64ToDecrypt);
            ArgCheck.NotNullOrEmpty(nameof(password), password);
            return dataProtector.DecryptData(base64ToDecrypt.FromBase64(urlEncoded), (encoding ?? _defaultEncoding).GetBytes(password));
        }
    }
}

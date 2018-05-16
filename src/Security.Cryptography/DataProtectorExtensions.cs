// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Security;
using System.Text;

namespace KodeAid.Security.Cryptography
{
    public static class DataProtectorExtensions
    {
        private static readonly Encoding _encoder = Encoding.UTF8;
        private static readonly SecureString _password = new SecureString();

        static DataProtectorExtensions()
        {
            _password.AppendChar('h');
            _password.AppendChar('*');
            _password.AppendChar('3');
            _password.AppendChar('M');
            _password.AppendChar('w');
            _password.AppendChar('&');
            _password.AppendChar('+');
            _password.AppendChar('K');
            _password.AppendChar('3');
            _password.AppendChar('!');
            _password.AppendChar('0');
            _password.AppendChar('s');
            _password.AppendChar('8');
            _password.AppendChar('#');
            _password.AppendChar('p');
            _password.AppendChar('J');
            _password.MakeReadOnly();
        }

        public static string EncryptStringToBase64(this IDataProtector dataProtector, string textToEncrypt)
        {
            return EncryptStringToBase64(dataProtector, textToEncrypt, _password.Unsecure());
        }

        public static string DecryptStringFromBase64(this IDataProtector dataProtector, string base64ToDecrypt)
        {
            return DecryptStringFromBase64(dataProtector, base64ToDecrypt, _password.Unsecure());
        }

        public static string EncryptDataToBase64(this IDataProtector dataProtector, byte[] dataToEncrypt)
        {
            return EncryptDataToBase64(dataProtector, dataToEncrypt, _password.Unsecure());
        }

        public static byte[] DecryptDataFromBase64(this IDataProtector dataProtector, string base64ToDecrypt)
        {
            return DecryptDataFromBase64(dataProtector, base64ToDecrypt, _password.Unsecure());
        }

        public static byte[] EncryptData(this IDataProtector dataProtector, byte[] dataToEncrypt)
        {
            ArgCheck.NotNull("dataProtector", dataProtector);
            return dataProtector.EncryptData(dataToEncrypt, _encoder.GetBytes(_password.Unsecure()));
        }

        public static byte[] DecryptData(this IDataProtector dataProtector, byte[] dataToDecrypt)
        {
            ArgCheck.NotNull("dataProtector", dataProtector);
            return dataProtector.DecryptData(dataToDecrypt, _encoder.GetBytes(_password.Unsecure()));
        }

        public static string EncryptStringToBase64(this IDataProtector dataProtector, string textToEncrypt, string password)
        {
            ArgCheck.NotNull("dataProtector", dataProtector);
            ArgCheck.NotNullOrEmpty("textToEncrypt", textToEncrypt);
            ArgCheck.NotNullOrEmpty("password", password);
            return EncryptDataToBase64(dataProtector, _encoder.GetBytes(textToEncrypt), password);
        }

        public static string DecryptStringFromBase64(this IDataProtector dataProtector, string base64ToDecrypt, string password)
        {
            ArgCheck.NotNull("dataProtector", dataProtector);
            ArgCheck.NotNullOrEmpty("base64ToDecrypt", base64ToDecrypt);
            ArgCheck.NotNullOrEmpty("password", password);
            return _encoder.GetString(DecryptDataFromBase64(dataProtector, base64ToDecrypt, password));
        }

        public static string EncryptDataToBase64(this IDataProtector dataProtector, byte[] dataToEncrypt, string password)
        {
            ArgCheck.NotNull("dataProtector", dataProtector);
            ArgCheck.NotNullOrEmpty("dataToEncrypt", dataToEncrypt);
            ArgCheck.NotNullOrEmpty("password", password);
            return Convert.ToBase64String(dataProtector.EncryptData(dataToEncrypt, _encoder.GetBytes(password)));
        }

        public static byte[] DecryptDataFromBase64(this IDataProtector dataProtector, string base64ToDecrypt, string password)
        {
            ArgCheck.NotNull("dataProtector", dataProtector);
            ArgCheck.NotNullOrEmpty("base64ToDecrypt", base64ToDecrypt);
            ArgCheck.NotNullOrEmpty("password", password);
            return dataProtector.DecryptData(Convert.FromBase64String(base64ToDecrypt), _encoder.GetBytes(password));
        }
    }
}

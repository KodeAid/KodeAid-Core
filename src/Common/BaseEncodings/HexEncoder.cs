// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Diagnostics.CodeAnalysis;

namespace KodeAid
{
    public static class HexEncoder
    {
        [return: NotNullIfNotNull(nameof(bytes))]
        public static string? EncodeBytes(byte[]? bytes)
        {
            if (bytes == null)
            {
                return null;
            }

#if NETSTANDARD
            var chars = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                chars[i * 2] = GetHexChar(bytes[i] >> 4);
                chars[(i * 2) + 1] = GetHexChar(bytes[i] & 0xF);
            }

            return new string(chars);
#else
            return Convert.ToHexString(bytes);
#endif
        }

        [return: NotNullIfNotNull(nameof(hexString))]
        public static byte[]? DecodeBytes(string? hexString)
        {
            if (hexString == null)
            {
                return null;
            }

#if NETSTANDARD
            if (hexString.Length % 2 != 0)
            {
                throw new FormatException("The input is not a valid hex string as its length is not a multiple of 2.");
            }

            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)((GetHexValue(hexString[i * 2]) << 4) | GetHexValue(hexString[(i * 2) + 1]));
            }

            return bytes;
#else
            return Convert.FromHexString(hexString);
#endif
        }

#if NETSTANDARD
        private static char GetHexChar(int value)
        {
            return (char)(value < 10 ? '0' + value : 'A' + value - 10);
        }

        private static int GetHexValue(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }

            if (c >= 'A' && c <= 'F')
            {
                return c - 'A' + 10;
            }

            if (c >= 'a' && c <= 'f')
            {
                return c - 'a' + 10;
            }

            throw new FormatException("The input is not a valid hex string as it contains a non-hex character.");
        }
#endif
    }
}

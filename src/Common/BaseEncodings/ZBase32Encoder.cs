// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Wiry.Base32;

namespace KodeAid
{
    public static class ZBase32Encoder
    {
        public static string EncodeBytes(byte[] bytes)
        {
            return Base32Encoding.ZBase32.GetString(bytes);
        }

        public static byte[] DecodeBytes(string base32String)
        {
            return Base32Encoding.ZBase32.ToBytes(base32String);
        }
    }
}

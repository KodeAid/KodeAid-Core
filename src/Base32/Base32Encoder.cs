// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Wiry.Base32;

namespace KodeAid
{
    public static class Base32Encoder
    {
        public static string EncodeBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            return Base32Encoding.Standard.GetString(bytes);
        }

        public static byte[] DecodeBytes(string base32String)
        {
            if (base32String == null)
            {
                return null;
            }

            return Base32Encoding.Standard.ToBytes(base32String);
        }
    }
}

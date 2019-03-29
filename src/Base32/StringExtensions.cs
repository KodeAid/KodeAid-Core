// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System
{
    public static class StringExtensions
    {
        public static byte[] FromBase32String(this string base32String)
        {
            return Base32Encoder.DecodeBytes(base32String);
        }

        public static byte[] FromZBase32String(this string zbase32String)
        {
            return ZBase32Encoder.DecodeBytes(zbase32String);
        }
    }
}

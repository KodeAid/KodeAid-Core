// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System
{
    public static class ByteExtensions
    {
        public static string ToBase32String(this byte[] bytes)
        {
            return Base32Encoder.EncodeBytes(bytes);
        }

        public static string ToZBase32String(this byte[] bytes)
        {
            return ZBase32Encoder.EncodeBytes(bytes);
        }
    }
}

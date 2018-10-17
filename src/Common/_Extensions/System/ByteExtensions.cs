// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System
{
    public static class ByteExtensions
    {
        public static string ToBase64(this byte[] data, bool urlEncoded = false)
        {
            return Base64Encoder.EncodeBytes(data, urlEncoded);
        }

        internal static string ToBase36(this byte[] data)
        {
            return Base36Encoder.EncodeBytes(data);
        }
    }
}

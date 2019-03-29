// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Text;
using KodeAid;

namespace System
{
    public static class ByteExtensions
    {
        public static string ToUtf8String(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string ToBase64String(this byte[] bytes)
        {
            return Base64Encoder.EncodeBytes(bytes);
        }

        [Obsolete("Use ToBase64Url() instead in KodeAid.Base64Url.dll")]
        public static string ToBase64String(this byte[] bytes, bool urlEncoded)
        {
            return Base64Encoder.EncodeBytes(bytes, urlEncoded);
        }

        internal static string ToBase36String(this byte[] bytes)
        {
            return Base36Encoder.EncodeBytes(bytes);
        }
    }
}

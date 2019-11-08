// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Text;
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

        public static string ToBase64String(this byte[] bytes)
        {
            return Base64Encoder.EncodeBytes(bytes);
        }

        [Obsolete("Use " + nameof(ToBase64String) + "() or " + nameof(ToBase64Url) + "() instead.")]
        public static string ToBase64String(this byte[] bytes, bool urlEncoded)
        {
            if (urlEncoded)
            {
                return bytes.ToBase64Url();
            }

            return bytes.ToBase64String();
        }

        public static string ToBase64Url(this byte[] bytes)
        {
            return Base64UrlEncoder.EncodeBytes(bytes);
        }

        public static string ToUtf8String(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        internal static string ToBase36String(this byte[] bytes)
        {
            return Base36Encoder.EncodeBytes(bytes);
        }
    }
}

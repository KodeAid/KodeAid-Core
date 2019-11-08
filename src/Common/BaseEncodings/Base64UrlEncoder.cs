// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.Extensions.WebEncoders.Sources;

namespace KodeAid
{
    public static class Base64UrlEncoder
    {
        public static string EncodeBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            return WebEncoders.Base64UrlEncode(bytes);
        }

        public static byte[] DecodeBytes(string base64Url)
        {
            if (base64Url == null)
            {
                return null;
            }

            return WebEncoders.Base64UrlDecode(base64Url);
        }
    }
}

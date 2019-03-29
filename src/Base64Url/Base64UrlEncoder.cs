// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.WebUtilities;

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

            return Base64UrlTextEncoder.Encode(bytes);
        }

        public static byte[] DecodeBytes(string base64Url)
        {
            if (base64Url == null)
            {
                return null;
            }

            return Base64UrlTextEncoder.Decode(base64Url);
        }
    }
}

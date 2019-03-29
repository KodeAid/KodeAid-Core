// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System
{
    public static class StringExtensions
    {
        public static byte[] FromBase64Url(this string base64Url)
        {
            return Base64UrlEncoder.DecodeBytes(base64Url);
        }
    }
}

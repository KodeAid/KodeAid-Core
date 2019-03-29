// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System
{
    public static class ByteExtensions
    {
        public static string ToBase64Url(this byte[] bytes)
        {
            return Base64UrlEncoder.EncodeBytes(bytes);
        }
    }
}

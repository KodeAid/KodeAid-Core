// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using KodeAid.Text.Base64;

namespace System
{
    public static class ByteExtensions
    {
        public static string ToBase64(this byte[] data, bool urlEncoded = false)
        {
            return Base64Encoder.EncodeBytes(data, urlEncoded);
        }
    }
}

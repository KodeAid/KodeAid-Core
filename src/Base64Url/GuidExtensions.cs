// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace System
{
    public static class GuidExtensions
    {
        public static string ToBase64Url(this Guid guid)
        {
            return guid.ToByteArray().ToBase64Url();
        }
    }
}

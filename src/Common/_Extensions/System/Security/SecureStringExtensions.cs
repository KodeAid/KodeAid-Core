// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Runtime.InteropServices;
using KodeAid;

namespace System.Security
{
    public static class SecureStringExtensions
    {
        public static string Unsecure(this SecureString secured)
        {
            ArgCheck.NotNull(nameof(secured), secured);
            var ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secured);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        public static SecureString Secure(this string unsecured)
        {
            ArgCheck.NotNull(nameof(unsecured), unsecured);
            var secureString = new SecureString();
            foreach (var c in unsecured)
                secureString.AppendChar(c);
            secureString.MakeReadOnly();
            return secureString;
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Caching
{
    public class CacheAccessException : ApplicationException
    {
        public CacheAccessException()
        {
        }

        public CacheAccessException(string message)
            : base(message)
        {
        }

        public CacheAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

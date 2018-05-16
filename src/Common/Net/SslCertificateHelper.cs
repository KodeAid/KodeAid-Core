// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net;

namespace KodeAid.Net
{
    public static class SslCertificateHelper
    {
        public static void TrustAllCertificates()
        {
            // ensure its cleared
            ServicePointManager.ServerCertificateValidationCallback = null;
            // not using += as we don't want to chain hundreds of the same callback
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Net;

namespace KodeAid.Net
{
    public static class DnsHelper
    {
        public static string GetFullyQualifiedDomainNameOfLocalhost()
        {
            return Dns.GetHostEntry("localhost").HostName;
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.Net.Http.Headers;

namespace KodeAid.AspNetCore.Mvc.Formatters.Internal
{
    internal static class MediaTypeHeaderValues
    {
        public static readonly MediaTypeHeaderValue TextPlain
            = MediaTypeHeaderValue.Parse("text/plain").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationXml
            = MediaTypeHeaderValue.Parse("application/xml").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue TextXml
            = MediaTypeHeaderValue.Parse("text/xml").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationAnyXmlSyntax
            = MediaTypeHeaderValue.Parse("application/*+xml").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationOctetStream
            = MediaTypeHeaderValue.Parse("application/octet-stream").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationFormUrlEncoded
            = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded").CopyAsReadOnly();
    }
}

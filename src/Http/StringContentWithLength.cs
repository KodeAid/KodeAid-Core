// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Text;

namespace System.Net.Http
{
    /// <summary>
    /// Ensures the content length header is set which does not happen automatically when using ASP.NET Core's Test Server.
    /// https://github.com/dotnet/aspnetcore/issues/18463
    /// </summary>
    public class StringContentWithLength : StringContent
    {
        public StringContentWithLength(string content)
            : base(content)
        {
            EnsureContentLength();
        }

        public StringContentWithLength(string content, Encoding encoding)
            : base(content, encoding)
        {
            EnsureContentLength();
        }

        public StringContentWithLength(string content, Encoding encoding, string mediaType)
            : base(content, encoding, mediaType)
        {
            EnsureContentLength();
        }

        public StringContentWithLength(string content, string unvalidatedContentType)
            : base(content)
        {
            Headers.TryAddWithoutValidation("Content-Type", unvalidatedContentType);
            EnsureContentLength();
        }

        private void EnsureContentLength()
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            // required due to https://github.com/dotnet/aspnetcore/issues/18463
            var contentLenth = Headers.ContentLength;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }
    }
}

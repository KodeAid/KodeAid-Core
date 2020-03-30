// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;

namespace KodeAid.AspNetCore.Http.Logging.Response
{
    public class ResponseLoggingOptions
    {
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 0 by default which disables body logging.
        /// Application Insights allows 32,768 bytes message size.
        /// </summary>
        public long MaxBodyByteCount { get; set; }

        public Func<HttpContext, bool> ShouldLog { get; set; }

        /// <summary>
        /// The name of the logger category.
        /// Will default to "KodeAid.AspNetCore.Http.Logging.Response" if null.
        /// </summary>
        public string LoggerName { get; set; }
    }
}

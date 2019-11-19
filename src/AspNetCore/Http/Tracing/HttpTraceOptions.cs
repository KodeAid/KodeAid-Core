// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace KodeAid.AspNetCore.Http.Tracing
{
    public class HttpTraceOptions
    {
        public HttpTraceMode RequestMode { get; set; } = HttpTraceMode.Enabled;

        public HttpTraceMode ResponseMode { get; set; } = HttpTraceMode.Enabled;

        /// <summary>
        /// URL path prefixes to not log.
        /// </summary>
        public List<string> IgnoredPathPrefixes { get; } = new List<string>();

        /// <summary>
        /// Set to override the default log level of <see cref="LogLevel.Trace"/> to which the logs are written to.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;
    }
}

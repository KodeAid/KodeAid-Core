// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.AspNetCore.Http.Tracing
{
    public enum HttpTraceMode
    {
        Disabled = 0,

        Enabled = 1,

        /// <summary>
        /// This will affect performance.
        /// </summary>
        IncludeBody = 2
    }
}

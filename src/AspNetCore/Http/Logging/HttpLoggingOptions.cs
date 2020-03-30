// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using KodeAid.AspNetCore.Http.Logging.Request;
using KodeAid.AspNetCore.Http.Logging.Response;

namespace KodeAid.AspNetCore.Http.Logging
{
    public class HttpLoggingOptions
    {
        public RequestLoggingOptions Request { get; } = new RequestLoggingOptions();
        public ResponseLoggingOptions Response { get; } = new ResponseLoggingOptions();
    }
}

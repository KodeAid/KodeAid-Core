// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;

#if NET8_0_OR_GREATER
using Asp.Versioning;
#else
using Microsoft.AspNetCore.Mvc;
#endif

namespace KodeAid.AspNetCore.Mvc.Versioning
{
    public class HttpApiVersionAccessor : IApiVersionAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpApiVersionAccessor(IHttpContextAccessor httpContextAccessor)
        {
            ArgCheck.NotNull(nameof(httpContextAccessor), httpContextAccessor);

            _httpContextAccessor = httpContextAccessor;
        }

        public ApiVersion GetRequestedApiVersion()
        {
            return _httpContextAccessor.HttpContext.GetRequestedApiVersion();
        }
    }
}

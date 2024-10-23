// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


#if NET8_0_OR_GREATER
using Asp.Versioning;
#else
using Microsoft.AspNetCore.Mvc;
#endif

namespace KodeAid.AspNetCore.Mvc.Versioning
{
    public interface IApiVersionAccessor
    {
        ApiVersion GetRequestedApiVersion();
    }
}

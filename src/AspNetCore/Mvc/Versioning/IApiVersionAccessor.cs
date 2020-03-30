// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc;

namespace KodeAid.AspNetCore.Mvc.Versioning
{
    public interface IApiVersionAccessor
    {
        ApiVersion GetRequestedApiVersion();
    }
}

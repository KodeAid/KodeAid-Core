// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



using Microsoft.AspNetCore.Mvc;

namespace KodeAid.AspNetCore.Mvc.Versioning
{
    public class ApiVersionAccessor : IApiVersionAccessor
    {
        private readonly ApiVersion _version;

        public ApiVersionAccessor(ApiVersion version)
        {
            ArgCheck.NotNull(nameof(version), version);

            _version = version;
        }

        public ApiVersion GetRequestedApiVersion()
        {
            return _version;
        }
    }
}

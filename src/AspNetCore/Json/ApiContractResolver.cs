// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.Serialization.Json.ContractResolvers;
using Microsoft.AspNetCore.Http;

namespace KodeAid.AspNetCore.Json
{
    public class ApiContractResolver : PredicateContractResolver
    {
        public ApiContractResolver(IHttpContextAccessor httpContextAccessor)
        {
            ArgCheck.NotNull(nameof(httpContextAccessor), httpContextAccessor);

            Add(new EmptyArrayPredicate());
            Add(new VersionedApiPredicate(httpContextAccessor));
        }
    }
}

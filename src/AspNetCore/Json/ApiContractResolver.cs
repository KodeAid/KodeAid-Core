// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.AspNetCore.Mvc.Versioning;
using KodeAid.Json.Serialization;

namespace KodeAid.AspNetCore.Json
{
    public class ApiContractResolver : PredicateContractResolver
    {
        public ApiContractResolver(IApiVersionAccessor versionAccessor)
        {
            ArgCheck.NotNull(nameof(versionAccessor), versionAccessor);

            PropertyPredicates.Add(new EmptyArrayPredicate());
            PropertyPredicates.Add(new ReadWritePredicate());
            PropertyPredicates.Add(new VersionedApiPredicate(versionAccessor));
        }
    }
}

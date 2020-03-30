// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using KodeAid.AspNetCore.Mvc.Versioning;
using KodeAid.Json.Serialization;

namespace KodeAid.AspNetCore.Json
{
    public class VersionedApiPredicate : VersionedModelPredicate
    {
        public VersionedApiPredicate(IApiVersionAccessor versionAccessor, IEqualityComparer<string> comparer = null)
            : base(() => versionAccessor.GetRequestedApiVersion()?.ToString(), comparer)
        {
        }
    }
}

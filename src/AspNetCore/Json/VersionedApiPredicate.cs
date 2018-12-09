// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using KodeAid.Serialization.Json.ContractResolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KodeAid.AspNetCore.Json
{
    public class VersionedApiPredicate : VersionedModelPredicate
    {
        public VersionedApiPredicate(IHttpContextAccessor httpContextAccessor, IEqualityComparer<string> comparer = null)
            : base(() => httpContextAccessor.HttpContext.GetRequestedApiVersion()?.ToString(), comparer)
        {
        }
    }
}

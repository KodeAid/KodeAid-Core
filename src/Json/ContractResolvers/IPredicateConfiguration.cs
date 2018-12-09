// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Serialization.Json.ContractResolvers
{
    public interface IPredicateConfiguration
    {
        Predicate<object> GetPredicate(MemberInfo member, JsonProperty property);
    }
}

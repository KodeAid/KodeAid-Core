// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public interface IPredicateConfiguration
    {
        Predicate<object> GetShouldSerializePredicate(MemberInfo member, JsonProperty property);
        Predicate<object> GetShouldDeserializePredicate(MemberInfo member, JsonProperty property);
    }
}

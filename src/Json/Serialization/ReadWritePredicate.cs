// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public class ReadWritePredicate : IPropertyPredicate
    {
        public Predicate<object> GetShouldSerializePredicate(MemberInfo member, JsonProperty property)
        {
            if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
            {
                var attributes = property.AttributeProvider.GetAttributes(true).OfType<JsonReadOnlyAttribute>().ToList();
                if (attributes != null && attributes.Count > 0)
                {
                    return obj => false;
                }
            }

            return null;
        }

        public Predicate<object> GetShouldDeserializePredicate(MemberInfo member, JsonProperty property)
        {
            if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
            {
                var attributes = property.AttributeProvider.GetAttributes(true).OfType<JsonWriteOnlyAttribute>().ToList();
                if (attributes != null && attributes.Count > 0)
                {
                    return obj => false;
                }
            }

            return null;
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public class ReadWriteContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property != null && (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property))
            {
                var attributes = property.AttributeProvider.GetAttributes(true);

                if (attributes.OfType<JsonReadOnlyAttribute>().Any())
                {
                    property.ShouldSerialize = instance => false;
                }

                if (attributes.OfType<JsonWriteOnlyAttribute>().Any())
                {
                    property.ShouldDeserialize = instance => false;
                }
            }

            return property;
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public class EmptyArrayContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property != null &&
                (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) &&
                typeof(ICollection).IsAssignableFrom(property.PropertyType))
            {
                if (property.AttributeProvider.GetAttributes(true).OfType<JsonEmptyArrayAttribute>().FirstOrDefault() is JsonEmptyArrayAttribute emptyArrayAttribute &&
                    emptyArrayAttribute.EmptyArrayHandling.HasFlag(EmptyArrayHandling.Ignore))
                {
                    var shouldSerialize = property.ShouldSerialize;
                    property.ShouldSerialize = instance =>
                    {
                        if (shouldSerialize != null && !shouldSerialize(instance))
                        {
                            return false;
                        }

                        return !((member.MemberType == MemberTypes.Field ? ((FieldInfo)member).GetValue(instance) : ((PropertyInfo)member).GetValue(instance))
                            is ICollection collection) || collection.Count > 0;
                    };
                }
            }

            return property;
        }
    }
}

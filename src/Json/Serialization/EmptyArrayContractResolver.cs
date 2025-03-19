// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections;
using System.Collections.Generic;
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

            if (property != null && (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) && IsCollectionType(property.PropertyType))
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

        private static bool IsCollectionType(Type type)
        {
            if (typeof(ICollection).IsAssignableFrom(type))
            {
                return true;
            }

            var interfaces = type.GetInterfaces().ToList();

            if (type.IsInterface)
            {
                interfaces.Insert(0, type);
            }

            return interfaces
                .Where(i => i.IsGenericType)
                .Select(i => i.IsGenericTypeDefinition ? i : i.GetGenericTypeDefinition())
                .Any(t => t == typeof(ICollection<>) || t == typeof(IReadOnlyCollection<>));
        }
    }
}

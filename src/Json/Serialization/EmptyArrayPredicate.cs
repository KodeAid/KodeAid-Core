// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public class EmptyArrayPredicate : PropertyPredicate
    {
        public override Predicate<object> GetShouldSerializePredicate(MemberInfo member, JsonProperty property)
        {
            if ((member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) && typeof(ICollection).IsAssignableFrom(property.PropertyType))
            {
                if (property.AttributeProvider.GetAttributes(true).OfType<JsonEmptyArrayAttribute>().FirstOrDefault() is JsonEmptyArrayAttribute emptyArrayAttribute &&
                    emptyArrayAttribute.EmptyArrayHandling.HasFlag(EmptyArrayHandling.Ignore))
                {
                    return obj =>
                    {
                        return !((member.MemberType == MemberTypes.Field ? ((FieldInfo)member).GetValue(obj) : ((PropertyInfo)member).GetValue(obj))
                            is ICollection collection) || collection.Count > 0;
                    };
                }
            }

            return null;
        }
    }
}

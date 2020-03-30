// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public class PredicateContractResolver : DefaultContractResolver
    {
        public PredicateContractResolver()
        {
        }

        public PredicateContractResolver(params IPropertyPredicate[] propertyPredicates)
            : this((IEnumerable<IPropertyPredicate>)propertyPredicates)
        {
        }

        public PredicateContractResolver(IEnumerable<IPropertyPredicate> propertyPredicates)
        {
            ArgCheck.NotNull(nameof(propertyPredicates), propertyPredicates);

            PropertyPredicates.AddRange(propertyPredicates.WhereNotNull());
        }

        public List<IPropertyPredicate> PropertyPredicates { get; } = new List<IPropertyPredicate>();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (PropertyPredicates.Count == 0)
            {
                return property;
            }

            // should serialize
            var predicates = PropertyPredicates.Select(c => c.GetShouldSerializePredicate(member, property)).WhereNotNull().ToList();
            if (predicates.Count > 0)
            {
                if (property.ShouldSerialize != null)
                {
                    predicates.Insert(0, property.ShouldSerialize);
                }

                property.ShouldSerialize = obj => predicates.All(p => p(obj));
            }

            // should deserialize
            predicates = PropertyPredicates.Select(c => c.GetShouldDeserializePredicate(member, property)).WhereNotNull().ToList();
            if (predicates.Count > 0)
            {
                if (property.ShouldDeserialize != null)
                {
                    predicates.Insert(0, property.ShouldDeserialize);
                }

                property.ShouldDeserialize = obj => predicates.All(p => p(obj));
            }

            return property;
        }
    }
}

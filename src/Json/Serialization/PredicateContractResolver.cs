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
        private readonly IList<IPredicateConfiguration> _predicateConfigurations = new List<IPredicateConfiguration>();

        public PredicateContractResolver()
        {
        }

        public PredicateContractResolver(params IPredicateConfiguration[] predicateConfigurations)
            : this((IEnumerable<IPredicateConfiguration>)predicateConfigurations)
        {
        }

        public PredicateContractResolver(IEnumerable<IPredicateConfiguration> predicateConfigurations)
        {
            ArgCheck.NotNull(nameof(predicateConfigurations), predicateConfigurations);
            _predicateConfigurations.AddRange(predicateConfigurations.WhereNotNull());
        }

        public void Add(IPredicateConfiguration predicateConfiguration)
        {
            ArgCheck.NotNull(nameof(predicateConfiguration), predicateConfiguration);
            _predicateConfigurations.Add(predicateConfiguration);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (_predicateConfigurations.Count == 0)
            {
                return property;
            }

            // should serialize

            var predicates = _predicateConfigurations.Select(c => c.GetShouldSerializePredicate(member, property)).WhereNotNull().ToList();
            if (predicates.Count > 0)
            {
                if (property.ShouldSerialize != null)
                {
                    predicates.Insert(0, property.ShouldSerialize);
                }

                property.ShouldSerialize = obj => predicates.All(p => p(obj));
            }

            // should deserialize
            predicates = _predicateConfigurations.Select(c => c.GetShouldDeserializePredicate(member, property)).WhereNotNull().ToList();
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

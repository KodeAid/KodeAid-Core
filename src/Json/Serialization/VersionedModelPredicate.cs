// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KodeAid.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public class VersionedModelPredicate : IPredicateConfiguration
    {
        private readonly Func<string> _getCurrentVersion;
        private readonly IEqualityComparer<string> _comparer;

        public VersionedModelPredicate(Func<string> getCurrentVersion, IEqualityComparer<string> comparer = null)
        {
            ArgCheck.NotNull(nameof(getCurrentVersion), getCurrentVersion);
            _getCurrentVersion = getCurrentVersion;
            _comparer = comparer ?? new DefaultComparer();
        }

        public Predicate<object> GetPredicate(MemberInfo member, JsonProperty property)
        {
            if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
            {
                var attributes = property.AttributeProvider.GetAttributes(true).OfType<VersionedPropertyAttribute>().ToList();
                if (attributes != null && attributes.Count > 0)
                {
                    var versions = attributes
                        .SelectMany(a => a.Versions.EmptyIfNull())
                        .WhereNotNull()
                        .ToSet(_comparer);

                    if (versions.Count > 0)
                    {
                        return obj =>
                        {
                            var version = _getCurrentVersion();
                            if (version == null)
                            {
                                return true;
                            }

                            return versions.Contains(version);
                        };
                    }
                }
            }

            return null;
        }

        public class DefaultComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                if (string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                if (double.TryParse(x, out var dx) && double.TryParse(y, out var dy) && dx == dy)
                {
                    return true;
                }

                if (Version.TryParse(x, out var vx) && Version.TryParse(y, out var vy) && vx == vy)
                {
                    return true;
                }

                return false;
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}

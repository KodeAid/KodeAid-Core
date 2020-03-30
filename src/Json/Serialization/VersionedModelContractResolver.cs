// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KodeAid.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KodeAid.Json.Serialization
{
    public class VersionedModelContractResolver : DefaultContractResolver
    {
        private readonly Func<Version> _getCurrentVersion;
        private readonly IEqualityComparer<Version> _comparer;

        public VersionedModelContractResolver(Func<Version> getCurrentVersion, IEqualityComparer<Version> comparer = null)
        {
            ArgCheck.NotNull(nameof(getCurrentVersion), getCurrentVersion);
            _getCurrentVersion = getCurrentVersion;
            _comparer = comparer;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property != null && (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property))
            {
                var attributes = property.AttributeProvider.GetAttributes(true).OfType<VersionedPropertyAttribute>().ToList();
                if (attributes != null && attributes.Count > 0)
                {
                    var versions = attributes
                        .OfType<VersionedPropertyAttribute>()
                        .SelectMany(a => a.Versions.EmptyIfNull())
                        .Select(s => Version.TryParse(s, out var v) ? v : null)
                        .WhereNotNull()
                        .ToSet(_comparer);

                    if (versions.Count > 0)
                    {
                        var shouldSerialize = property.ShouldSerialize;
                        property.ShouldSerialize = instance =>
                        {
                            if (shouldSerialize != null && !shouldSerialize(instance))
                            {
                                return false;
                            }

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

            return property;
        }
    }
}

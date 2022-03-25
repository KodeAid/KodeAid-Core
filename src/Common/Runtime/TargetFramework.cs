// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace KodeAid.Runtime
{
    public sealed class TargetFramework
    {
        public TargetFramework(Assembly assembly)
        {
            ArgCheck.NotNull(nameof(assembly), assembly);

            var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

            if (targetFrameworkAttribute == null)
            {
                throw new ArgumentException($"Assembly {assembly.FullName} is missing the TargetFrameworkAttribute.", nameof(assembly));
            }

            Name = targetFrameworkAttribute.FrameworkName?.Split(',').FirstOrDefault();
            Version = targetFrameworkAttribute.FrameworkName?.Split(',')
                .Skip(1)
                .Where(p => p.StartsWith("Version="))
                .Select(v => v.Split('=').Skip(1).FirstOrDefault()?.Trim().TrimStart('v').TrimToNull())
                .WhereNotNull()
                .Select(v => Version.Parse(v))
                .FirstOrDefault();
            DisplayName = targetFrameworkAttribute.FrameworkDisplayName?.TrimToNull() ??
                (this.IsNetCoreApp() ? (Version.Major >= 5 ? $".NET {Version}" : $".NET Core {Version}") :
                 this.IsNetFramework() ? $".NET Framework {Version}" :
                 this.IsNetStandard() ? $".NET Standard {Version}" :
                 $"{Name} {Version}");
        }

        /// <summary>
        /// Get the target framework of the entry assembly.
        /// </summary>
        public static TargetFramework Current { get; } = new TargetFramework(Assembly.GetEntryAssembly());
        public string Name { get; }
        public Version Version { get; }
        public string DisplayName { get; }
    }
}

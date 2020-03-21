// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace KodeAid.Runtime
{
    public static class TargetFrameworkHelper
    {
        static TargetFrameworkHelper()
        {
            var targetFrameworkAttribute = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>();
            Name = targetFrameworkAttribute?.FrameworkName?.Split(',').FirstOrDefault();
            Version = targetFrameworkAttribute?.FrameworkName?.Split(',').Skip(1).Where(p => p.StartsWith("Version=")).Select(v => v.Split('=').Skip(1).FirstOrDefault()?.Trim().TrimStart('v').TrimToNull()).WhereNotNull().Select(v => Version.Parse(v)).FirstOrDefault();
            IsNetCoreApp = (Name?.IndexOf("NETCoreApp", StringComparison.OrdinalIgnoreCase) ?? -1) > -1;
            IsNetFramework = (Name?.IndexOf("NETFramework", StringComparison.OrdinalIgnoreCase) ?? -1) > -1;
            DisplayName = targetFrameworkAttribute?.FrameworkDisplayName?.TrimToNull() ?? (IsNetCoreApp ? $".NET Core {Version.ToString()}" : IsNetFramework ? $".NET Framework {Version.ToString()}" : null);
        }

        public static string Name { get; }
        public static Version Version { get; }
        public static string DisplayName { get; }
        public static bool IsNetCoreApp { get; }
        public static bool IsNetFramework { get; }
    }
}

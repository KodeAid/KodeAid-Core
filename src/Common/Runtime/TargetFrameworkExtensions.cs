// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Runtime
{
    public static class TargetFrameworkExtensions
    {
        public static bool IsNetCoreApp(this TargetFramework targetFramework) => (targetFramework.Name?.IndexOf("NETCoreApp", StringComparison.OrdinalIgnoreCase) ?? -1) > -1;
        public static bool IsNetFramework(this TargetFramework targetFramework) => (targetFramework.Name?.IndexOf("NETFramework", StringComparison.OrdinalIgnoreCase) ?? -1) > -1;
        public static bool IsNetStandard(this TargetFramework targetFramework) => (targetFramework.Name?.IndexOf("NETStandard", StringComparison.OrdinalIgnoreCase) ?? -1) > -1;
    }
}

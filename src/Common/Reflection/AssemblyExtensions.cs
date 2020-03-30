// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KodeAid.Reflection
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the types defined in this assembly that can be loaded.
        /// </summary>
        /// <param name="assembly">The assembly to search.</param>
        /// <returns>An array that contains all the types that are defined in this assembly that can be loaded.</returns>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            ArgCheck.NotNull(nameof(assembly), assembly);

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.WhereNotNull();
            }
        }
    }
}

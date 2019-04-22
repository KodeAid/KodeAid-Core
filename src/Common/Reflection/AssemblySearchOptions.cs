// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Reflection
{
    [Flags]
    public enum AssemblySearchOptions
    {
        None = 0,

        /// <summary>
        /// Include all referenced assemblies.
        /// </summary>
        ReferencedAssemblies = 1,

        /// <summary>
        /// Search the directory of the starting point assembly.
        /// </summary>
        StartingDirectory = 2,

        /// <summary>
        /// Search the directories of all the referenced assemblies that matched and were included.
        /// </summary>
        AssemblyDirectories = 4,

        /// <summary>
        /// Include all subdirectories in any directory searching.
        /// </summary>
        IncludeSubdirectories = 8,

        /// <summary>
        /// The default options if not specified.
        /// </summary>
        Default = ReferencedAssemblies | StartingDirectory | AssemblyDirectories,
    }
}

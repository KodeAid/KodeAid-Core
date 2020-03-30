// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using KodeAid.AutoMapper;
using KodeAid.Reflection;

namespace AutoMapper
{
    public static class MapperConfigurationExtensions
    {
        /// <summary>
        /// Find and add profiles from <see cref="IMappingRegistration"/> definitions found, optionally including those not loaded.
        /// </summary>
        /// <param name="configuration">The mapping configuration.</param>
        /// <param name="startingPoint">Assemblies to start the search from, if not provided the entry assembly is used: Assembly.GetEntryAssembly().</param>
        /// <param name="assemblySearchOptions">How to search for additional assemblies to include.</param>
        /// <param name="assemblyNamePrefixes">Case insensitive prefixes of assembly names and file names (*.dlls) to include in search, null/empty to include all.</param>
        public static IMapperConfigurationExpression AddRegisteredProfiles(this IMapperConfigurationExpression configuration, Assembly startingPoint = null, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, params string[] assemblyNamePrefixes)
        {
            var registrationTypes = ReflectionHelper.FindAllTypes<IMappingRegistration>(startingPoint: startingPoint, assemblySearchOptions: assemblySearchOptions, throwOnError: throwOnError, assemblyNamePrefixes: assemblyNamePrefixes);

            foreach (var registrationType in registrationTypes)
            {
                var registration = (IMappingRegistration)Activator.CreateInstance(registrationType);
                registration.Register(configuration);
            }

            return configuration;
        }

        /// <summary>
        /// Find and add profiles from search assemblies, optionally including those not loaded.
        /// </summary>
        /// <param name="configuration">The mapping configuration.</param>
        /// <param name="startingPoint">Assemblies to start the search from, if not provided the entry assembly is used: Assembly.GetEntryAssembly().</param>
        /// <param name="assemblySearchOptions">How to search for additional assemblies to include.</param>
        /// <param name="assemblyNamePrefixes">Case insensitive prefixes of assembly names and file names (*.dlls) to include in search, null/empty to include all.</param>
        public static IMapperConfigurationExpression AddProfilesFromAssemblies(this IMapperConfigurationExpression configuration, Assembly startingPoint = null, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, params string[] assemblyNamePrefixes)
        {
            var profileTypes = ReflectionHelper.FindAllTypes<Profile>(startingPoint: startingPoint, assemblySearchOptions: assemblySearchOptions, throwOnError: throwOnError, assemblyNamePrefixes: assemblyNamePrefixes);

            foreach (var profileType in profileTypes)
            {
                configuration.AddProfile(profileType);
            }

            return configuration;
        }
    }
}

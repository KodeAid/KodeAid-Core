// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using KodeAid.DependencyInjection;
using KodeAid.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Find and register services from <see cref="IServiceRegistration"/> definitions found, optionally including those not loaded.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="startingPoint">Assemblies to start the search from, if not provided the entry assembly is used: Assembly.GetEntryAssembly().</param>
        /// <param name="assemblySearchOptions">How to search for additional assemblies to include.</param>
        /// <param name="assemblyNamePrefixes">Case insensitive prefixes of assembly names and file names (*.dlls) to include in search, null/empty to include all.</param>
        public static void AddRegisteredServices(this IServiceCollection services, Assembly startingPoint = null, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, params string[] assemblyNamePrefixes)
        {
            var registrationTypes = ReflectionHelper.FindAllTypes<IServiceRegistration>(startingPoint: startingPoint, assemblySearchOptions: assemblySearchOptions, throwOnError: throwOnError, assemblyNamePrefixes: assemblyNamePrefixes);
            foreach (var registrationType in registrationTypes)
            {
                var registration = (IServiceRegistration)Activator.CreateInstance(registrationType);
                registration.Register(services);
            }
        }
    }
}

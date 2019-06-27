// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KodeAid.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Assembly startingPoint = null, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, IEnumerable<string> assemblyNamePrefixes = null)
        {
            var profileTypes = ReflectionHelper.FindAllTypes<Profile>(startingPoint: startingPoint, assemblySearchOptions: assemblySearchOptions, throwOnError: throwOnError, assemblyNamePrefixes: assemblyNamePrefixes);
            return services.AddAutoMapper(profileTypes.ToArray());
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction, Assembly startingPoint = null, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, IEnumerable<string> assemblyNamePrefixes = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            var profileTypes = ReflectionHelper.FindAllTypes<Profile>(startingPoint: startingPoint, assemblySearchOptions: assemblySearchOptions, throwOnError: throwOnError, assemblyNamePrefixes: assemblyNamePrefixes);
            return services.AddAutoMapper(configAction, profileTypes, serviceLifetime);
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IServiceProvider, IMapperConfigurationExpression> configAction, Assembly startingPoint = null, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, IEnumerable<string> assemblyNamePrefixes = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            var profileTypes = ReflectionHelper.FindAllTypes<Profile>(startingPoint: startingPoint, assemblySearchOptions: assemblySearchOptions, throwOnError: throwOnError, assemblyNamePrefixes: assemblyNamePrefixes);
            return services.AddAutoMapper(configAction, profileTypes, serviceLifetime);
        }
    }
}

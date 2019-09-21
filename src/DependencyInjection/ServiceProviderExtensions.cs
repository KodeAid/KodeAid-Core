// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Instantiate a type with constructor arguments provided directly and/or from an System.IServiceProvider.
        /// </summary>
        /// <typeparam name="T">The type to activate.</typeparam>
        /// <param name="provider">The service provider used to resolve dependencies.</param>
        /// <param name="parameters">Constructor arguments not provided by the provider.</param>
        /// <returns>An activated object of type T.</returns>
        public static T CreateInstance<T>(this IServiceProvider provider, object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<T>(provider, parameters);
        }

        /// <summary>
        /// Instantiate a type with constructor arguments provided directly and/or from an System.IServiceProvider and initialize it.
        /// </summary>
        /// <typeparam name="T">The type to activate.</typeparam>
        /// <param name="provider">The service provider used to resolve dependencies.</param>
        /// <param name="init">The intialization to perform on the activated object.</param>
        /// <param name="parameters">Constructor arguments not provided by the provider.</param>
        /// <returns>An activated object of type T and initialized via <paramref name="init"/>.</returns>
        public static T CreateInstance<T>(this IServiceProvider provider, object[] parameters, Action<T> init)
        {
            var instance = provider.CreateInstance<T>(parameters);

            if (instance != null && init != null)
            {
                init(instance);
            }

            return instance;
        }

        /// <summary>
        /// Retrieve an instance of the given type from the service provider.
        /// If one is not found then instantiate it with constructor arguments provided directly and/or from an System.IServiceProvider.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <param name="provider">The service provider used to resolve dependencies.</param>
        /// <param name="parameters">Constructor arguments not provided by the provider.</param>
        /// <returns>The resolved service or created instance of type T.</returns>
        public static T GetServiceOrCreateInstance<T>(this IServiceProvider provider, object[] parameters)
        {
            return provider.GetServiceOrCreateInstance<T>(parameters, null);
        }

        /// <summary>
        /// Retrieve an instance of the given type from the service provider.
        /// If one is not found then instantiate it with constructor arguments provided directly and/or from an System.IServiceProvider.
        /// Then initialize it.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <param name="provider">The service provider used to resolve dependencies.</param>
        /// <param name="init">The intialization to perform on the resolved service or created instance, where the first argument is True if an instance was created.</param>
        /// <param name="parameters">Constructor arguments not provided by the provider.</param>
        /// <returns>The resolved service or created instance of type T, and initialized via <paramref name="init"/>.</returns>
        public static T GetServiceOrCreateInstance<T>(this IServiceProvider provider, object[] parameters, Action<bool, T> init)
        {
            var created = false;
            var service = provider.GetService<T>();

            if (service == null)
            {
                service = provider.CreateInstance<T>(parameters);
                created = true;
            }

            if (service != null && init != null)
            {
                init(created, service);
            }

            return service;
        }

        /// <summary>
        /// Get service of type T from the System.IServiceProvider and initialize it.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="provider">The System.IServiceProvider to retrieve the service object from.</param>
        /// <param name="init">The intialization to perform on the service object.</param>
        /// <returns>A service object of type T.</returns>
        /// <exception cref="InvalidOperationException">There is no service of type T.</exception>
        public static T GetRequiredService<T>(this IServiceProvider provider, Action<T> init)
        {
            var service = provider.GetRequiredService<T>();

            if (service != null && init != null)
            {
                init(service);
            }

            return service;
        }

        /// <summary>
        /// Get service of type T from the System.IServiceProvider and initialize it.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="provider">The System.IServiceProvider to retrieve the service object from.</param>
        /// <param name="init">The intialization to perform on the service object.</param>
        /// <returns>A service object of type T or null if there is no such service.</returns>
        public static T GetService<T>(this IServiceProvider provider, Action<T> init)
        {
            var service = provider.GetService<T>();

            if (service != null && init != null)
            {
                init(service);
            }

            return service;
        }
    }
}

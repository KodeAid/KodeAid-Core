// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Get service of type T from the System.IServiceProvider and initialize it.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="provider">The System.IServiceProvider to retrieve the service object from.</param>
        /// <param name="init">The intialization action to perform if a non-null service object is found.</param>
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
        /// <param name="init">The intialization action to perform if a non-null service object is found.</param>
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

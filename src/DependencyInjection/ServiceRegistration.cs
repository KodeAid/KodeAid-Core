// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.Extensions.DependencyInjection;

namespace KodeAid.DependencyInjection
{
    public abstract class ServiceRegistration : IServiceRegistration
    {
        protected IServiceCollection Services { get; private set; }

        protected abstract void Register();

        void IServiceRegistration.Register(IServiceCollection services)
        {
            ArgCheck.NotNull(nameof(services), services);

            Services = services;

            Register();
        }
    }
}

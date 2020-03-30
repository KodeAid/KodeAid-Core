// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.Extensions.DependencyInjection;

namespace KodeAid.DependencyInjection
{
    public interface IServiceRegistration
    {
        void Register(IServiceCollection services);
    }
}

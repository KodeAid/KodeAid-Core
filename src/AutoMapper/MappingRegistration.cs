// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Reflection;
using AutoMapper;

namespace KodeAid.AutoMapper
{
    public abstract class MappingRegistration : IMappingRegistration
    {
        protected IMapperConfigurationExpression Configuration { get; private set; }

        protected abstract void Register();

        protected void AddProfile<TMappingProfile>()
            where TMappingProfile : Profile, new()
        {
            Configuration.AddProfile<TMappingProfile>();
        }

        protected void AddProfile(Type profileType)
        {
            Configuration.AddProfile(profileType);
        }

        protected void AddProfile(Profile profile)
        {
            Configuration.AddProfile(profile);
        }

        protected void AddMapsFromAssembly()
        {
            Configuration.AddMaps(Assembly.GetExecutingAssembly());
        }

        void IMappingRegistration.Register(IMapperConfigurationExpression configuration)
        {
            ArgCheck.NotNull(nameof(configuration), configuration);

            Configuration = configuration;

            Register();
        }
    }
}

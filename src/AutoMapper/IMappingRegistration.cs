// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using AutoMapper;

namespace KodeAid.AutoMapper
{
    public interface IMappingRegistration
    {
        void Register(IMapperConfigurationExpression configuration);
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace KodeAid.AutoMapper
{
    public abstract class AssemblyMappingRegistration : MappingRegistration
    {
        protected override void Register()
        {
            AddMapsFromAssembly();
        }
    }
}

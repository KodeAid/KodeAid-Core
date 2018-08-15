// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Serialization.Json.ContractResolvers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class JsonEmptyArrayAttribute : Attribute
    {
        public EmptyArrayHandling EmptyArrayHandling { get; set; }
    }
}

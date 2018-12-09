// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class VersionedPropertyAttribute : Attribute
    {
        public VersionedPropertyAttribute(params string[] versions)
        {
            Versions = versions;
        }

        public string[] Versions { get; set; }
    }
}
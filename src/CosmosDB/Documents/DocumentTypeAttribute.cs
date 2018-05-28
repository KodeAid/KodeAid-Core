// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Azure.Cosmos.Documents
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class DocumentTypeAttribute : Attribute
    {
        public DocumentTypeAttribute(string type)
        {
            Type = type;
        }

        public string Type { get; set; }
    }
}

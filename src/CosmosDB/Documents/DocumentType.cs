// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Newtonsoft.Json;

namespace KodeAid.Azure.Cosmos.Documents
{
    internal class DocumentType
    {
        public const string DocumentTypeJsonPropertyName = "_type";

        [JsonProperty(DocumentTypeJsonPropertyName)]
        public string Type { get; set; }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Azure.Storage
{
    public sealed class AzureBlobStorageClientOptions
    {
        public string ConnectionString { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string SharedAccessSignature { get; set; }
        public string EndpointSuffix { get; set; }
        public string ContainerName { get; set; }
        public string DefaultDirectoryRelativeAddress { get; set; }
        public bool UseDefaultAzureCredential { get; set; }
        public bool UseSnapshots { get; set; }
        public TimeSpan? LeaseDuration { get; set; }
        public TimeSpan? NetworkTimeout { get; set; }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using KodeAid.Security.Secrets;
using Microsoft.WindowsAzure.Storage;

namespace KodeAid.Azure.Storage
{
    public sealed class AzureBlobStorageClientOptions
    {
        /// <summary>
        /// 60 seconds.
        /// </summary>
        public static readonly TimeSpan DefaultLeaseDuration = TimeSpan.FromSeconds(60);

        public CloudStorageAccount StorageAccount { get; set; }

        public string ConnectionString { get; set; }

        public string SharedAccessSignature { get; set; }

        public string AccountName { get; set; }

        public string EndpointSuffix { get; set; }

        public ISecretReadOnlyStore SecretStore { get; set; }

        public string ConnectionStringSecretName { get; set; }

        public string SharedAccessSignatureSecretName { get; set; }

        public string ContainerName { get; set; }

        public string DefaultDirectoryRelativeAddress { get; set; }

        public TimeSpan? LeaseDuration { get; set; } = DefaultLeaseDuration;

        public bool UseSnapshots { get; set; }
    }
}

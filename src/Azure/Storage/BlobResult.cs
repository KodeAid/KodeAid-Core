// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;

namespace KodeAid.Azure.Storage
{
    public class BlobResult
    {
        public string BlobName { get; set; }
        public string DirectoryRelativeAddress { get; set; }
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public string ETag { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public IDictionary<string, string> Metadata { get; } = new Dictionary<string, string>();
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace KodeAid.Storage
{
    public interface IBlobMeta : IStoreMeta
    {
        string ContentType { get; set; }
        string ContentEncoding { get; set; }
        IDictionary<string, string> Metadata { get; }
    }
}

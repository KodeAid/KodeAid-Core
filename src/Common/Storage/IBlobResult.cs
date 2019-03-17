// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.IO;

namespace KodeAid.Storage
{
    public interface IBlobResult : IStoreResult
    {
        Stream Content { get; set; }
        string ContentType { get; set; }
        string ContentEncoding { get; set; }
        IDictionary<string, string> Metadata { get; }
    }
}

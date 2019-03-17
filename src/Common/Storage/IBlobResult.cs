// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;

namespace KodeAid.Storage
{
    public interface IBlobResult : IBlobMeta, IStoreResult
    {
        Stream Content { get; set; }
    }
}

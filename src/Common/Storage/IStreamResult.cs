// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



using System;
using System.IO;

namespace KodeAid.Storage
{
    public interface IStreamResult : IStoreResult, IDisposable
    {
        Stream Stream { get; }
    }
}

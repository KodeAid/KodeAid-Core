// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KodeAid.Serialization
{
    public interface IAsyncStreamSerializer
    {
        Task SerializeToStreamAsync(Stream stream, object graph, CancellationToken cancellationToken = default);
        Task<object> DeserializeFromStreamAsync(Type type, Stream stream, CancellationToken cancellationToken = default);
    }
}

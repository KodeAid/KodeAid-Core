// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid.Serialization;

namespace KodeAid.Json
{
    public interface IJsonSerializer : IStringSerializer, IStreamSerializer, IAsyncStreamSerializer
    {
    }
}

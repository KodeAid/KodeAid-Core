// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Caching
{
    public interface ICacheResult<T>
    {
        string Key { get; }
        T Value { get; }
        bool IsHit { get; }
        DateTimeOffset? LastUpdated { get; }
    }
}

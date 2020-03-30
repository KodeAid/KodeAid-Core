// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
        TimeZoneInfo TimeZone { get; }
        DateTimeOffset UtcNow { get; }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Data
{
    public interface ICreatedTime
    {
        DateTimeOffset? CreatedAt { get; set; }
    }
}

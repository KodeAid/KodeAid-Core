// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;

namespace KodeAid.Collections.Generic
{
    public sealed class CaseInsensitiveSet : HashSet<string>
    {
        public CaseInsensitiveSet()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}

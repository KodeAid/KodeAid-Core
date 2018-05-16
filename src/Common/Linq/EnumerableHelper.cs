// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace KodeAid.Linq
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> From<T>(T item)
        {
            return new List<T>() { item };
        }

        public static IEnumerable<T> From<T>(params T[] items)
        {
            return new List<T>(items);
        }
    }
}
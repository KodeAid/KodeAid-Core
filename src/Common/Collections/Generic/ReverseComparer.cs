// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace KodeAid.Collections.Generic
{
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> _comparer;

        public ReverseComparer(IComparer<T> comparer)
        {
            ArgCheck.NotNull(nameof(comparer), comparer);
            _comparer = comparer;
        }

        public int Compare(T left, T right)
        {
            return _comparer.Compare(right, left);
        }
    }
}

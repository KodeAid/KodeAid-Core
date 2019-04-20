// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;

namespace KodeAid
{
    public static class Enum<T>
         where T : struct, IConvertible
    {
        public static bool IsSingleBitFlagSet(T value)
        {
            var i = value.ToInt32(null);
            return i.IsPowerOfTwo();
        }

        public static int GetNumberOfFlagsSet(T value, bool singleBitFlagsOnly = false)
        {
            var i = value.ToInt32(null);

            if (i == 0)
            {
                return 0;
            }

            return GetFlagsSet(value, singleBitFlagsOnly).Count();
        }

        public static IEnumerable<T> GetFlagsSet(T value, bool singleBitFlagsOnly = false)
        {
            var i = value.ToInt32(null);

            if (i != 0)
            {
                foreach (var v in GetValues(includeZero: false, singleBitValuesOnly: singleBitFlagsOnly)
                    .Select(e => e.ToInt32(null))
                    .Where(e => ((e & i) == e))
                    .Cast<T>())
                {
                    yield return v;
                }
            }
        }

        public static IEnumerable<T> GetValues(bool includeZero = false, bool singleBitValuesOnly = false)
        {
            foreach (var v in Enum.GetValues(typeof(T))
                .Cast<T>()
                .Distinct()
                .Where(e => includeZero || e.ToInt32(null) != 0)
                .Where(e => !singleBitValuesOnly || (includeZero && e.ToInt32(null) == 0) || IsSingleBitFlagSet(e)))
            {
                yield return v;
            }
        }
    }
}

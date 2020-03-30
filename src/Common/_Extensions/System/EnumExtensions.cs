// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using KodeAid;

namespace System
{
    public static class EnumExtensions
    {
        public static bool HasFlagSet<TEnum>(this TEnum value, TEnum flag)
            where TEnum : Enum
        {
            return value.HasFlag(flag);
        }

        public static bool IsSingleBitSet<TEnum>(this TEnum value)
            where TEnum : Enum, IConvertible
        {
            var i = value.ToInt32(null);
            return i.IsPowerOfTwo();
        }

        public static int GetNumberOfFlagsSet<TEnum>(this TEnum value, bool singleBitFlagsOnly = false)
            where TEnum : struct, Enum, IConvertible
        {
            var i = value.ToInt32(null);

            if (i == 0)
            {
                return 0;
            }

            return value.GetFlagsSet(singleBitFlagsOnly).Count();
        }

        public static IEnumerable<TEnum> GetFlagsSet<TEnum>(this TEnum value, bool singleBitFlagsOnly = false)
            where TEnum : struct, Enum, IConvertible
        {
            var i = value.ToInt32(null);

            if (i != 0)
            {
                foreach (var v in EnumHelper.GetValues<TEnum>(includeZero: false, singleBitValuesOnly: singleBitFlagsOnly)
                    .Select(e => e.ToInt32(null))
                    .Where(e => ((e & i) == e))
                    .Cast<TEnum>())
                {
                    yield return v;
                }
            }
        }

        public static string ToString<TEnum>(this TEnum value, bool useEnumMemberAttribute)
            where TEnum : struct, Enum, IFormattable
        {
            return value.ToString(useEnumMemberAttribute, null);
        }

        public static string ToString<TEnum>(this TEnum value, bool useEnumMemberAttribute, string format)
            where TEnum : struct, Enum, IFormattable
        {
            if (useEnumMemberAttribute)
            {
                return EnumHelper.GetSerializedName(value);
            }

            return value.ToString(format);
        }
    }
}

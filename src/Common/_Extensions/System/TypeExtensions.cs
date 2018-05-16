// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using KodeAid;

namespace System
{
    public static class TypeExtensions
    {
        public static bool IsNumeric(this Type type)
        {
            return IsIntegralNumeric(type) || IsDecimalPrecisionNumeric(type);
        }

        public static bool IsIntegralNumeric(this Type type)
        {
            ArgCheck.NotNull(nameof(type), type);
            if (type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(int) || type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong))
            {
                return true;
            }
            return false;
        }

        public static bool IsDecimalPrecisionNumeric(this Type type)
        {
            return IsFloatingPointNumeric(type) || type == typeof(decimal);
        }

        public static bool IsFloatingPointNumeric(this Type type)
        {
            ArgCheck.NotNull(nameof(type), type);
            return type == typeof(float) || type == typeof(double);
        }
    }
}

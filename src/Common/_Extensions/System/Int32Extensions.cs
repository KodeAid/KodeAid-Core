// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



namespace System
{
    public static class Int32Extensions
    {
        public static bool IsPowerOfTwo(this int i)
        {
            if (i == 0)
            {
                return false;
            }

            return (i & (i - 1)) == 0;
        }

        public static int GetNumberOfBitsSet(this int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
    }
}

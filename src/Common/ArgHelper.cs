// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    public static class ArgHelper
    {
        public static int Clamp(int value, int minValue, int maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }
    }
}

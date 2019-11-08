// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace Wiry.Base32
{
    /// <summary>
    /// https://github.com/wiry-net/Wiry.Base32/blob/master/src/Wiry.Base32/LookupTable.cs
    /// </summary>
    internal sealed class LookupTable
    {
        public int LowCode { get; }
        public int[] Values { get; }

        public LookupTable(int lowCode, int[] values)
        {
            LowCode = lowCode;
            Values = values;
        }
    }
}
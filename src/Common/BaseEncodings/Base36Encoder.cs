// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Numerics;
using System.Text;

namespace KodeAid
{
    public static class Base36Encoder
    {
        private const string _codepage = "0123456789abcdefghijklmnopqrstuvwxyz";
        private static readonly BigInteger _codepageLength = new BigInteger(_codepage.Length);

        internal static string EncodeBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            var dividend = new BigInteger(bytes);

            // this isn't possible due to MSB bytes of value 0 being dropped as they add no value to the BigInteger
            // we also lose the sign of the number so sometehing like the below could help that but there must be a cleaner way

            //if (dividend < BigInteger.Zero)
            //{
            //    dividend = new BigInteger(BigInteger.Abs(dividend).ToByteArray().Concat(new byte[] { 2 }).ToArray());
            //}
            //else
            //{
            //    dividend = new BigInteger(data.Concat(new byte[] { 1 }).ToArray());
            //}

            var sb = new StringBuilder();
            while (!dividend.IsZero)
            {
                dividend = BigInteger.DivRem(dividend, _codepageLength, out var remainder);
                sb.Insert(0, _codepage[Math.Abs((int)remainder)]);
            }

            return sb.ToString();
        }

        internal static byte[] DecodeBytes(string base36String)
        {
            if (base36String == null)
            {
                return null;
            }

            if (base36String.Length == 0)
            {
                throw new FormatException("Invalid Base36 format.");
            }

            var product = new BigInteger();
            for (var c = 0; c < base36String.Length; c++)
            {
                var i = _codepage.IndexOf(base36String[c]);
                if (i < 0)
                {
                    throw new FormatException("Invalid Base36 format.");
                }
                product *= _codepageLength;
                product += i;
            }

            return product.ToByteArray();

            //var data = product.ToByteArray();
            //if (data.Length == 0)
            //{
            //    throw new FormatException("Invalid Base36 format.");
            //}
            //if (data[data.Length - 1] == 2)
            //{
            //    return (new BigInteger(data.Take(data.Length - 1).ToArray()) * -1).ToByteArray();
            //}
            //else if (data[data.Length - 1] == 1)
            //{
            //    return new BigInteger(data.Take(data.Length - 1).ToArray()).ToByteArray();
            //}
            //throw new FormatException("Invalid Base36 format.");
        }

        /// <summary>
        /// Creates a new <see cref="Guid"/> encoded as a base 36 string.
        /// </summary>
        /// <returns></returns>
        public static string NewGuid()
        {
            return Guid.NewGuid().ToBase36String();
        }
    }
}

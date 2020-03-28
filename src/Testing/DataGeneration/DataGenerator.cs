// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Text;

namespace KodeAid.Testing.DataGeneration
{
    public class DataGenerator : IDataGenerator
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        public DateTime Now { get; set; } = DateTime.Now;

        public DateTime Today { get => Now.Date; }

        public bool GetBoolean()
        {
            return _random.Next(0, 2) == 1;
        }

        public char GetCharacter(string characterSet)
        {
            return characterSet[GetInteger(0, characterSet.Length - 1)];
        }

        public DateTime GetDate(DateTime minimum, DateTime maximum)
        {
            return minimum.AddDays(GetInteger(0, (int)((maximum - minimum).TotalDays)));
        }

        public decimal GetDecimal(decimal minimum, decimal maximum, int precision = 2)
        {
            return decimal.Round(((decimal)_random.NextDouble() * (maximum - minimum)) + minimum, precision);
        }

        public int GetInteger(int minimum, int maximum)
        {
            return _random.Next(minimum, maximum + 1);
        }

        public string GetString(int minimumLength, int maximumLength, string characterSet = null)
        {
            var sb = new StringBuilder();

            var length = GetInteger(minimumLength, maximumLength);

            for (var i = 0; i < length; ++i)
            {
                if (!string.IsNullOrEmpty(characterSet))
                {
                    sb.Append(GetCharacter(characterSet));
                }
                else
                {
                    sb.Append((char)_random.Next(33, 127));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a string from a mask.
        /// 'A' is upper case letter.
        /// 'a' is lower case letter.
        /// '0' is number.
        /// 'X' is upper case alphanumeric.
        /// 'x' is lower case alphanumeric.
        /// All other characters are copied as is.
        /// </summary>
        public string GetString(string mask)
        {
            if (mask == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            foreach (var c in mask)
            {
                if (c == 'X')
                {
                    sb.Append(GetCharacter("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
                }
                else if (c == 'x')
                {
                    sb.Append(GetCharacter("0123456789abcdefghijklmnopqrstuvwxyz"));
                }
                else if (c == 'A')
                {
                    sb.Append(GetCharacter("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
                }
                else if (c == 'a')
                {
                    sb.Append(GetCharacter("abcdefghijklmnopqrstuvwxyz"));
                }
                else if (c == '0')
                {
                    sb.Append(GetInteger(0, 9));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}

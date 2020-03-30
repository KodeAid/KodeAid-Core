// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Testing.DataGeneration
{
    public interface IDataGenerator
    {
        DateTime Now { get; }
        DateTime Today { get; }

        bool GetBoolean();
        char GetCharacter(string characterSet);
        DateTime GetDate(DateTime minimum, DateTime maximum);
        decimal GetDecimal(decimal minimum, decimal maximum, int precision = 2);
        int GetInteger(int minimum, int maximum);
        string GetString(int minimumLength, int maximumLength, string characterSet = null);

        /// <summary>
        /// Gets a string from a mask.
        /// 'A' is upper case letter.
        /// 'a' is lower case letter.
        /// '0' is number.
        /// 'Z' is upper case alphanumeric.
        /// 'z' is lower case alphanumeric.
        /// All other characters are copied as is.
        /// </summary>
        string GetString(string mask);
    }
}

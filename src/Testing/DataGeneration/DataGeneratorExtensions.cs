// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KodeAid.Testing.DataGeneration
{
    public static class DataGeneratorExtensions
    {
        public static string GetBooleanString(this IDataGenerator dataGenerator, string trueValue, string falseValue)
        {
            return dataGenerator.GetBoolean() ? trueValue : falseValue;
        }

        public static string GetNumber(this IDataGenerator dataGenerator, int numberOfDigits)
        {
            return dataGenerator.GetString(numberOfDigits, numberOfDigits, "0123456789");
        }

        public static string ChooseString(this IDataGenerator dataGenerator, Type constantsClass, int nullOdds = 0)
        {
            ArgCheck.NotNull(nameof(constantsClass), constantsClass);

            var values = constantsClass.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsPublic && f.IsStatic && f.IsLiteral)
                .Select(f => f.GetRawConstantValue()?.ToString())
                .ToList();

            return dataGenerator.ChooseString(values, nullOdds);
        }

        public static string ChooseString(this IDataGenerator dataGenerator, IEnumerable<string> values, int nullOdds = 0)
        {
            if (dataGenerator.IsNull(nullOdds))
            {
                return null;
            }

            return values.ElementAt(dataGenerator.GetInteger(0, values.Count() - 1));
        }

        public static string ChooseString(this IDataGenerator dataGenerator, params string[] values)
        {
            return values[dataGenerator.GetInteger(0, values.Length - 1)];
        }

        public static DateTime GetPastDate(this IDataGenerator dataGenerator, int maximumYearsInThePast, int minimumYearsInThePast = 0)
        {
            var minYear = dataGenerator.Today.Year - maximumYearsInThePast;
            var maxYear = dataGenerator.Today.Year - minimumYearsInThePast;
            var month = dataGenerator.Today.Month;
            var day = dataGenerator.Today.Day;

            return dataGenerator.GetDate(
                new DateTime(minYear, month, Math.Min(day, DateTime.DaysInMonth(minYear, month))),
                new DateTime(maxYear, month, Math.Min(day, DateTime.DaysInMonth(maxYear, month))));
        }

        public static DateTime GetFutureDate(this IDataGenerator dataGenerator, int maximumYearsInTheFuture, int minimumYearsInTheFuture = 0)
        {
            var maxYear = dataGenerator.Today.Year + maximumYearsInTheFuture;
            var minYear = dataGenerator.Today.Year + minimumYearsInTheFuture;
            var month = dataGenerator.Today.Month;
            var day = dataGenerator.Today.Day;

            return dataGenerator.GetDate(
                new DateTime(minYear, month, Math.Min(day, DateTime.DaysInMonth(minYear, month))),
                new DateTime(maxYear, month, Math.Min(day, DateTime.DaysInMonth(maxYear, month))));
        }

        public static string GetString(this IDataGenerator dataGenerator, int length, string characterSet)
        {
            return dataGenerator.GetString(length, length, characterSet);
        }

        public static string GetString(this IDataGenerator dataGenerator, int length, bool allowSpaces = false)
        {
            return dataGenerator.GetString(length, length, allowSpaces);
        }

        public static string GetString(this IDataGenerator dataGenerator, int minimumLength, int maximumLength, bool allowSpaces = false)
        {
            return dataGenerator.GetString(minimumLength, maximumLength, allowSpaces ? "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz " : "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
        }

        public static string GetWord(this IDataGenerator dataGenerator, int minimumLength, int maximumLength)
        {
            var word = dataGenerator.GetString(minimumLength, maximumLength, "abcdefghijklmnopqrstuvwxyz");
            return char.ToUpper(word[0]).ToString() + word.Substring(1);
        }

        public static string GetWords(this IDataGenerator dataGenerator, int minimumLength, int maximumLength)
        {
            var words = dataGenerator.GetString(minimumLength, maximumLength, "abcdefghijklmnopqrstuvwxyz     ").CollapseAndTrim();

            if (words.Length < minimumLength)
            {
                words += dataGenerator.GetString(minimumLength - words.Length, minimumLength - words.Length, "abcdefghijklmnopqrstuvwxyz");
            }

            return char.ToUpper(words[0]).ToString() + words.Substring(1);
        }

        public static string GetFirstName(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return dataGenerator.ChooseString(DataGenerationSets.FirstNames, nullOdds);
        }

        public static string GetMiddleName(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return dataGenerator.ChooseString(DataGenerationSets.MiddleNames, nullOdds);
        }

        public static string GetLastName(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return dataGenerator.ChooseString(DataGenerationSets.LastNames, nullOdds);
        }

        public static string GetFullName(this IDataGenerator dataGenerator, bool withMiddleName = false, int nullOdds = 0)
        {
            if (dataGenerator.IsNull(nullOdds))
            {
                return null;
            }

            if (withMiddleName)
            {
                return $"{dataGenerator.GetFirstName()} {dataGenerator.GetMiddleName()} {dataGenerator.GetLastName()}";
            }

            return $"{dataGenerator.GetFirstName()} {dataGenerator.GetLastName()}";
        }

        public static string GetCompanyName(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return dataGenerator.ChooseString(DataGenerationSets.CompanyNames, nullOdds);
        }

        public static string GetPhoneNumber(this IDataGenerator dataGenerator, bool formatted = false, string prefix = "+1")
        {
            if (formatted)
            {
                return $"{prefix} ({dataGenerator.GetNumber(3)}) {dataGenerator.GetNumber(3)}-{dataGenerator.GetNumber(4)}";
            }

            return $"{prefix}{dataGenerator.GetNumber(10)}";
        }

        public static string GetDomainName(this IDataGenerator dataGenerator, string companyName = null, int nullOdds = 0)
        {
            if (dataGenerator.IsNull(nullOdds))
            {
                return null;
            }

            companyName = LowercaseLettersOnly(companyName);

            if (companyName != null)
            {
                return companyName + ".test";
            }

            return dataGenerator.ChooseString(DataGenerationSets.DomainNames);
        }

        public static string GetWebsite(this IDataGenerator dataGenerator, string companyName = null, int nullOdds = 0)
        {
            var domainName = dataGenerator.GetDomainName(companyName, nullOdds);

            if (domainName == null)
            {
                return null;
            }

            return $"www.{domainName}";
        }

        public static string GetEmail(this IDataGenerator dataGenerator, string firstName = null, string lastName = null, string companyName = null, int nullOdds = 0)
        {
            var domainName = dataGenerator.GetDomainName(companyName, nullOdds);

            if (domainName == null)
            {
                return null;
            }

            var emailName = ($"{LowercaseLettersOnly(firstName)}.{LowercaseLettersOnly(lastName)}".Trim('.').TrimToNull() ?? dataGenerator.GetWord(5, 20));

            return $"{emailName}@{domainName}";
        }

        public static string GetStreetAddress(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return $"{dataGenerator.GetInteger(1, 9999)} {dataGenerator.GetStreetName()}";
        }

        public static string GetStreetName(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            var streetName = dataGenerator.ChooseString(DataGenerationSets.StreetNames, nullOdds);

            if (streetName != null)
            {
                streetName += $" {dataGenerator.ChooseString(DataGenerationSets.StreetTypes)}";
            }

            return streetName;
        }

        public static string GetCityName(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return dataGenerator.ChooseString(DataGenerationSets.CityNames, nullOdds);
        }

        public static string GetProvinceCode(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return dataGenerator.ChooseString(DataGenerationSets.ProvinceCodes, nullOdds);
        }

        public static string GetProvinceName(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return dataGenerator.ChooseString(DataGenerationSets.ProvinceNames, nullOdds);
        }

        public static string GetPostalCode(this IDataGenerator dataGenerator, bool formatted = false, int nullOdds = 0)
        {
            if (formatted)
            {
                return dataGenerator.GetString("A0A 0A0");
            }

            return dataGenerator.GetString("A0A0A0");
        }

        public static string GetZipCode(this IDataGenerator dataGenerator, int nullOdds = 0)
        {
            return dataGenerator.GetNumber(5);
        }

        public static string GetSocialInsuranceNumber(this IDataGenerator dataGenerator, bool formatted = false, int nullOdds = 0)
        {
            if (formatted)
            {
                return dataGenerator.GetString("000-000-000");
            }

            return dataGenerator.GetNumber(9);
        }

        public static string GetSocialSecurityNumber(this IDataGenerator dataGenerator, bool formatted = false, int nullOdds = 0)
        {
            if (formatted)
            {
                return dataGenerator.GetString("000-00-0000");
            }

            return dataGenerator.GetNumber(9);
        }

        private static bool IsNull(this IDataGenerator dataGenerator, int nullOdds)
        {
            if (nullOdds <= 0)
            {
                return false;
            }

            return dataGenerator.GetInteger(1, nullOdds) == nullOdds;
        }

        private static string LowercaseLettersOnly(string str)
        {
            return Regex.Replace(str?.ToLowerInvariant() ?? string.Empty, @"[^a-z]", string.Empty, RegexOptions.Compiled).TrimToNull();
        }
    }
}

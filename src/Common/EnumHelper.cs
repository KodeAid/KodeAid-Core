// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace KodeAid
{
    public static class EnumHelper
    {
        public static IEnumerable<TEnum> GetValues<TEnum>(bool includeZero = false, bool singleBitValuesOnly = false)
            where TEnum : Enum, IConvertible
        {
            foreach (var v in Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Distinct()
                .Where(e => includeZero || e.ToInt32(null) != 0)
                .Where(e => !singleBitValuesOnly || (includeZero && e.ToInt32(null) == 0) || e.IsSingleBitSet()))
            {
                yield return v;
            }
        }

        /// <summary>
        /// Normalizes an string representation of an enumeration by replacing any occurences of <see cref="EnumMemberAttribute"/> with their corresponding constant name.
        /// </summary>
        public static string NormalizeName<TEnum>(string str, bool ignoreCase = false, bool ignoreWhitespace = false, string flagsDelimiter = ", ")
            where TEnum : Enum, IConvertible
        {
            return NormalizeName(Enum<TEnum>.Info, str, ignoreCase, ignoreWhitespace, flagsDelimiter);
        }

        /// <summary>
        /// Normalizes an string representation of an enumeration by replacing any occurences of <see cref="EnumMemberAttribute"/> with their corresponding constant name.
        /// </summary>
        public static string NormalizeName(Type enumType, string str, bool ignoreCase = false, bool ignoreWhitespace = false, string flagsDelimiter = ", ")
        {
            return NormalizeName(GetEnumInfo(enumType), str, ignoreCase, ignoreWhitespace, flagsDelimiter);
        }

        public static string GetSerializedName<TEnum>(TEnum value)
            where TEnum : Enum, IConvertible
        {
            var name = Enum<TEnum>.Info.Names.FirstOrDefault(n => value.CompareTo(n.Value) == 0);
            return name.SerializedName ?? name.Name;
        }

        private static string NormalizeName(EnumInfo enumInfo, string str, bool ignoreCase, bool ignoreWhitespace, string flagsDelimiter)
        {
            if (str == null)
            {
                return null;
            }

            if (ignoreWhitespace)
            {
                str = str.Trim();
            }

            str = str.Normalize();

            if (enumInfo.HasSerializedNames)
            {
                var hasFlags = enumInfo.EnumType.GetCustomAttribute<FlagsAttribute>() != null;

                if (hasFlags)
                {
                    if (flagsDelimiter == null)
                    {
                        flagsDelimiter = string.Empty;
                    }

                    foreach (var (Value, Name, SerializedName) in enumInfo.Names.Where(n => n.SerializedName != null))
                    {
                        if (ignoreWhitespace)
                        {
                            var delimiter = Regex.Escape(flagsDelimiter.Trim());
                            str = Regex.Replace(str, $@"(^|{delimiter})\s*{Regex.Escape(SerializedName.Trim())}\s*({delimiter}|$)", $"$1{Name}$3", RegexOptions.Compiled | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
                        }
                        else
                        {
                            var delimiter = Regex.Escape(flagsDelimiter);
                            str = Regex.Replace(str, $@"(^|{delimiter}){Regex.Escape(SerializedName)}({delimiter}|$)", $"$1{Name}$3", RegexOptions.Compiled | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
                        }
                    }

                    if (ignoreWhitespace)
                    {
                        str = str.Replace(flagsDelimiter.Trim(), flagsDelimiter);
                    }
                }
                else
                {
                    str = enumInfo.Names.FirstOrDefault(n => string.Equals(str, n.SerializedName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)).Name ?? str;
                }
            }

            return str;
        }

        private static EnumInfo GetEnumInfo(Type enumType)
        {
            return (EnumInfo)typeof(Enum<>).MakeGenericType(enumType).GetField(nameof(Enum<TypeCode>.Info), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        }

        private struct EnumInfo
        {
            public Type EnumType;
            public bool HasFlags;
            public bool HasSerializedNames;
            public List<(object Value, string Name, string SerializedName)> Names;
        }

        private static class Enum<TEnum>
            where TEnum : Enum, IConvertible
        {
            public static readonly EnumInfo Info;

            static Enum()
            {
                Info = new EnumInfo()
                {
                    EnumType = typeof(TEnum),
                    HasFlags = typeof(TEnum).GetCustomAttribute<FlagsAttribute>() != null,
                    Names = typeof(TEnum)
                       .GetFields(BindingFlags.Public | BindingFlags.Static)
                       .Where(f => f.IsLiteral)
                       .Select(f => new { Value = Enum.ToObject(typeof(TEnum), f.GetRawConstantValue()), Attribute = f.GetCustomAttribute<EnumMemberAttribute>() })
                       .Select(p => (p.Value, Enum.GetName(typeof(TEnum), p.Value), (p.Attribute?.IsValueSetExplicitly).GetValueOrDefault() ? p.Attribute.Value : null))
                       .ToList(),
                };

                Info.HasSerializedNames = Info.Names.Any(n => n.SerializedName != null);
            }
        }
    }
}

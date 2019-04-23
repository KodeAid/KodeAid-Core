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
            where TEnum : struct, Enum, IConvertible
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
        public static string NormalizeName<TEnum>(string value, bool ignoreCase = false, bool ignoreWhitespace = false, string flagsDelimiter = ", ")
            where TEnum : struct, Enum
        {
            return NormalizeName(Enum<TEnum>.Info, value, ignoreCase, ignoreWhitespace, flagsDelimiter);
        }

        /// <summary>
        /// Normalizes an string representation of an enumeration by replacing any occurences of <see cref="EnumMemberAttribute"/> with their corresponding constant name.
        /// </summary>
        public static string NormalizeName(Type enumType, string value, bool ignoreCase = false, bool ignoreWhitespace = false, string flagsDelimiter = ", ")
        {
            return NormalizeName(GetEnumInfo(enumType), value, ignoreCase, ignoreWhitespace, flagsDelimiter);
        }

        /// <summary>
        /// Get a string name of the enum, this will be the serialized name 
        /// if one was explicitly set via an <see cref="EnumMemberAttribute"/> attribute,
        /// otherwise it will be the normal enum field name.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <param name="value">The enumeration constant value.</param>
        /// <returns>The value's serialized name if it has one, otherwise the normal enum field name.</returns>
        public static string GetSerializedName<TEnum>(TEnum value)
            where TEnum : struct, Enum
        {
            var (Value, Name, SerializedName) = Enum<TEnum>.Info.Names.FirstOrDefault(n => value.CompareTo(n.Value) == 0);
            return SerializedName ?? Name;
        }

        /// <summary>
        /// Converts the string representation of the name, serialized name, or numeric value 
        /// of one or more enumerated constants to an equivalent enumerated object. 
        /// A parameter specifies whether the operation is case-insensitive.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type to which to convert value.</typeparam>
        /// <param name="value">The string representation of the enumeration name, serialized name, or underlying value to convert.</param>
        /// <param name="ignoreCase">true to ignore case; false to consider case.</param>
        /// <returns>An object of type enumType whose value is represented by value.</returns>
        public static TEnum Parse<TEnum>(string value, bool ignoreCase = false)
            where TEnum : struct, Enum
        {
            if (Enum.TryParse<TEnum>(value, ignoreCase, out var result))
            {
                return result;
            }

            value = NormalizeName<TEnum>(value, ignoreCase, true);
            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }

        /// <summary>
        /// Converts the string representation of the name, serialized name, or numeric value 
        /// of one or more enumerated constants to an equivalent enumerated object. 
        /// A parameter specifies whether the operation is case-insensitive.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type to which to convert value.</typeparam>
        /// <param name="value">The string representation of the enumeration name, serialized name, or underlying value to convert.</param>
        /// <param name="ignoreCase">true to ignore case; false to consider case.</param>
        /// <param name="result">When this method returns, result contains an object of type TEnum whose value
        /// is represented by value if the parse operation succeeds. If the parse operation
        /// fails, result contains the default value of the underlying type of TEnum. Note
        /// that this value need not be a member of the TEnum enumeration. This parameter
        /// is passed uninitialized.</param>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse<TEnum>(string value, out TEnum result, bool ignoreCase = false)
            where TEnum : struct, Enum
        {
            if (Enum.TryParse(value, ignoreCase, out result))
            {
                return true;
            }

            value = NormalizeName<TEnum>(value, ignoreCase, true);
            return Enum.TryParse(value, ignoreCase, out result);
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

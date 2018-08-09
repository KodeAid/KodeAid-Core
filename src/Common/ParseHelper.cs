// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace KodeAid
{
    public static class ParseHelper
    {
        private static readonly string[] _trueStringsForBooleanParsing = new[] { "true", "t", "1", "y", "yes", "on" };
        private static readonly string[] _falseStringsForBooleanParsing = new[] { "false", "f", "0", "n", "no", "off" };

        public static T Parse<T>(string str, bool ignoreCase = false)
        {
            return (T)Parse(str, typeof(T), ignoreCase: ignoreCase);
        }

        public static object Parse(string str, Type targetType, bool ignoreCase = false)
        {
            if (TryParse(str, targetType, out var value, ignoreCase: ignoreCase))
                return value;
            throw new ArgumentException($"Parameter {nameof(str)} cannot be parsed as type {targetType.FullName}.", "strValue");
        }

        public static T ParseOrDefault<T>(string str, bool ignoreCase = false, T defaultValue = default)
        {
            return (T)ParseOrDefault(str, typeof(T), ignoreCase, defaultValue);
        }

        public static object ParseOrDefault(string str, Type targetType, bool ignoreCase = false, object defaultValue = null)
        {
            ArgCheck.NotNull(nameof(targetType), targetType);
            if (defaultValue != null && !targetType.IsAssignableFrom(defaultValue.GetType()))
                throw new ArgumentException($"Parameter '{nameof(defaultValue)}' must be assignable to '{nameof(targetType)}'.", nameof(defaultValue));
            if (TryParse(str, targetType, out var value, ignoreCase: ignoreCase))
                return value;
            return defaultValue;
        }

        public static bool TryParse<T>(string str, out T value, bool ignoreCase = false)
        {
            if (TryParse(str, typeof(T), out var objValue, ignoreCase: ignoreCase))
            {
                value = (T)objValue;
                return true;
            }
            value = default;
            return false;
        }

        public static bool TryParse(string str, Type targetType, out object value, bool ignoreCase = false)
        {
            ArgCheck.NotNull(nameof(targetType), targetType);

            if (targetType.IsInterface || targetType.IsAbstract)  // static classes are abstract (and sealed)
            {
                throw new ArgumentException($"Parameter '{nameof(targetType)}' must be a struct or a concrete instance-based class (no interfaces and no abstract or static classes).", nameof(targetType));
            }

            // if null we can't do anything with it, return failure
            if (str == null)
            {
                value = null;
                return false;
            }

            // if wanting type string, just return the original
            if (targetType == typeof(string))
            {
                value = str;
                return true;
            }

            // if blank or just whitespace we can't do anything with it, return failure
            if (string.IsNullOrWhiteSpace(str))
            {
                value = null;
                return false;
            }

            // remove any nullable wrapping
            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // for booleans, allow case-insensitive values
            if (targetType == typeof(bool))
            {
                str = str.Trim().Normalize().ToLowerInvariant();
                if (_trueStringsForBooleanParsing.Contains(str))
                {
                    value = true;
                    return true;
                }
                if (_falseStringsForBooleanParsing.Contains(str))
                {
                    value = false;
                    return true;
                }

                // failed to convert to boolean
                value = null;
                return false;
            }

            if (targetType == typeof(DateTime))
            {
                if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var d))
                {
                    value = d;
                    return true;
                }
            }

            if (targetType == typeof(DateTimeOffset))
            {
                if (DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var d))
                {
                    value = d;
                    return true;
                }
            }

            // Type - examples:
            // "System.String"
            // "System.String, mscorlib"
            // "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            if (targetType == typeof(Type))
            {
                value = Type.GetType(str);
                return true;
            }

            if (targetType == typeof(Version))
            {
                if (Version.TryParse(str, out var version))
                {
                    value = version;
                    return true;
                }
                value = null;
                return false;
            }

            if (targetType.GetTypeInfo().IsEnum)
            {
                str = str.Trim().Normalize();

                // EnumMemberAttribute
                value = targetType.GetFields(BindingFlags.Public | BindingFlags.Static)
                    .SingleOrDefault(f =>
                    {
                        if (!f.IsLiteral)
                            return false;
                        var a = f.GetCustomAttribute<EnumMemberAttribute>(true);
                        if (a == null || !a.IsValueSetExplicitly)
                            return false;
                        if (!string.Equals(str, a.Value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                            return false;
                        return true;
                    })?.GetRawConstantValue();

                if (value != null)
                {
                    value = Enum.ToObject(targetType, value);
                    return true;
                }

                try
                {
                    // enum name or underlying integral value
                    value = Enum.Parse(targetType, str, ignoreCase);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            }

            try
            {
                var converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null)
                {
                    value = converter.ConvertFromString(str);
                    return true;
                }
            }
            catch { }

            try
            {
                // fallback is to use Convert.ChangeType()
                value = Convert.ChangeType(str, targetType);
                return true;
            }
            catch { }

            // failed to parse
            value = null;
            return false;
        }
    }
}

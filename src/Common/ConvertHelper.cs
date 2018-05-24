// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace KodeAid
{
    public static class ConvertHelper
    {
        private static readonly string[] _trueStringsForBooleanConversion = new[] { "true", "t", "1", "1.0", "y", "yes", "on", "ok" };
        private static readonly string[] _falseStringsForBooleanConversion = new[] { "false", "f", "0", "0.0", "n", "no", "off", "" };

        public static T Convert<T>(object value)
        {
            return (T)Convert(value, typeof(T));
        }

        public static object Convert(object value, Type targetType)
        {
            if (TryConvert(value, targetType, out var result))
                return result;
            // calling this again just to throw an error
            return System.Convert.ChangeType(value, targetType);
        }

        public static T ConvertOrDefault<T>(object value, T defaultValue = default)
        {
            return (T)ConvertOrDefault(value, typeof(T), defaultValue);
        }

        public static object ConvertOrDefault(object value, Type targetType, object defaultValue = null)
        {
            ArgCheck.NotNull(nameof(targetType), targetType);
            if (defaultValue != null && !targetType.IsAssignableFrom(defaultValue.GetType()))
                throw new ArgumentException($"Parameter '{nameof(defaultValue)}' must be assignable to '{nameof(targetType)}'.", nameof(defaultValue));
            if (TryConvert(value, targetType, out var result))
                return result;
            return defaultValue;
        }

        public static bool TryConvert<T>(object value, out T result)
        {
            if (TryConvert(value, typeof(T), out var r))
            {
                result = (T)r;
                return true;
            }
            result = default;
            return false;
        }

        public static bool TryConvert(object value, Type targetType, out object result)
        {
            ArgCheck.NotNull(nameof(targetType), targetType);

            try
            {
                if (value == null)
                {
                    result = default;
                    if (targetType.GetTypeInfo().IsValueType)
                        return false;
                    return true;
                }
                var sourceType = value.GetType();
                sourceType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
                targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
                if (sourceType == targetType || targetType.IsAssignableFrom(sourceType))
                {
                    result = value;
                    return true;
                }
                if (value is string && string.IsNullOrEmpty((string)value))
                {
                    result = default;
                    return false;
                }
                if (targetType == typeof(bool) && TryConvertBoolean(value, out var b))
                {
                    result = b;
                    return true;
                }
                var typeConverter = TypeDescriptor.GetConverter(targetType);
                if (typeConverter.CanConvertFrom(sourceType))
                {
                    result = typeConverter.ConvertFrom(value);
                    return true;
                }
                result = System.Convert.ChangeType(value, targetType);
                return true;
            }
            catch { }

            // failed to convert
            result = default;
            return false;
        }

        private static bool TryConvertBoolean(object value, out bool result)
        {
            if (value == null)
            {
                result = false;
                return true;
            }
            if (value is bool)
            {
                result = (bool)value;
                return true;
            }
            if (value is bool?)
            {
                result = ((bool?)value).GetValueOrDefault();
                return true;
            }
            if (_trueStringsForBooleanConversion.Contains(value.ToString().ToLowerInvariant()))
            {
                result = true;
                return true;
            }
            if (_falseStringsForBooleanConversion.Contains(value.ToString().ToLowerInvariant()))
            {
                result = false;
                return true;
            }
            result = default;
            return false;
        }
    }
}

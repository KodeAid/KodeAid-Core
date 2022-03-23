// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace KodeAid
{
    [DebuggerStepThrough]
    public static class ArgCheck
    {
        public static void NotNull(string paramName, [ValidatedNotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void NotNullOrDefault<T>(string paramName, [ValidatedNotNull] T value)
        {
            NotNull(paramName, value);

            if (value.Equals(default(T)))
            {
                throw new ArgumentException($"Parameter {paramName} cannot be {default(T)}.", paramName);
            }
        }

        public static void NotNullOrEmpty(string paramName, [ValidatedNotNull] string value)
        {
            NotNull(paramName, value);

            if (value.Length == 0)
            {
                throw new ArgumentException($"Parameter {paramName} cannot be an empty string.", paramName);
            }
        }

        public static void NotNullOrWhitespace(string paramName, [ValidatedNotNull] string value)
        {
            NotNull(paramName, value);

            if (value.Length == 0)
            {
                throw new ArgumentException($"Parameter {paramName} cannot be an empty string.", paramName);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Parameter {paramName} cannot be whitespace only.", paramName);
            }
        }

        public static void NotNullOrEmpty<T>(string paramName, [ValidatedNotNull] ICollection<T> value)
        {
            NotNull(paramName, value);

            if (value.Count == 0)
            {
                throw new ArgumentException($"Parameter {paramName} cannot be empty.", paramName);
            }
        }

        public static void NotNullOrEmpty<T>(string paramName, [ValidatedNotNull] IEnumerable<T> value)
        {
            NotNull(paramName, value);

            if (!value.Any())
            {
                throw new ArgumentException($"Parameter {paramName} cannot be empty.", paramName);
            }
        }

        public static void NotNullOrEmpty(string paramName, [ValidatedNotNull] IEnumerable value)
        {
            NotNull(paramName, value);

            if (!value.Cast<object>().Any())
            {
                throw new ArgumentException($"Parameter {paramName} cannot be empty.", paramName);
            }
        }

        public static void NotEqualTo<T>(string paramName, [ValidatedNotNull] IEquatable<T> value, T unequalValue, string unequalValueName = null)
        {
            NotNull(paramName, value);

            if (value.Equals(unequalValue))
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} cannot be {(unequalValueName ?? unequalValue.ToString())}.");
            }
        }

        public static void GreaterThan(string paramName, [ValidatedNotNull] IComparable value, object exclusiveMinimum, string exclusiveMinimumName = null)
        {
            NotNull(paramName, value);

            if (value.CompareTo(exclusiveMinimum) <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be greater than {(exclusiveMinimumName ?? exclusiveMinimum)}.");
            }
        }

        public static void GreaterThanOrEqualTo(string paramName, [ValidatedNotNull] IComparable value, object inclusiveMinimum, string inclusiveMinimumName = null)
        {
            NotNull(paramName, value);

            if (value.CompareTo(inclusiveMinimum) < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be greater than or equal to {(inclusiveMinimumName ?? inclusiveMinimum)}.");
            }
        }

        public static void LessThan(string paramName, [ValidatedNotNull] IComparable value, object exclusiveMaximum, string exclusiveMaximumName = null)
        {
            NotNull(paramName, value);

            if (value.CompareTo(exclusiveMaximum) >= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be less than {(exclusiveMaximumName ?? exclusiveMaximum)}.");
            }
        }

        public static void LessThanOrEqualTo(string paramName, [ValidatedNotNull] IComparable value, object inclusiveMaximum, string inclusiveMaximumName = null)
        {
            NotNull(paramName, value);

            if (value.CompareTo(inclusiveMaximum) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be less than or equal to {(inclusiveMaximumName ?? inclusiveMaximum)}.");
            }
        }

        public static void NotEqualTo(string paramName, [ValidatedNotNull] IComparable value, object unequalValue, string unequalValueName = null)
        {
            NotNull(paramName, value);

            if (value.CompareTo(unequalValue) == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must not be equal to {(unequalValueName ?? unequalValue)}.");
            }
        }

        public static void GreaterThan<T>(string paramName, [ValidatedNotNull] T value, T exclusiveMinimum, string exclusiveMinimumName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);

            if (value.CompareTo(exclusiveMinimum) <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be greater than {(exclusiveMinimumName ?? exclusiveMinimum.ToString())}.");
            }
        }

        public static void GreaterThanOrEqualTo<T>(string paramName, [ValidatedNotNull] T value, T inclusiveMinimum, string inclusiveMinimumName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);

            if (value.CompareTo(inclusiveMinimum) < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be greater than or equal to {(inclusiveMinimumName ?? inclusiveMinimum.ToString())}.");
            }
        }

        public static void LessThan<T>(string paramName, [ValidatedNotNull] T value, T exclusiveMaximum, string exclusiveMaximumName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);

            if (value.CompareTo(exclusiveMaximum) >= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be less than {(exclusiveMaximumName ?? exclusiveMaximum.ToString())}.");
            }
        }

        public static void LessThanOrEqualTo<T>(string paramName, [ValidatedNotNull] T value, T inclusiveMaximum, string inclusiveMaximumName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);

            if (value.CompareTo(inclusiveMaximum) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be less than or equal to {(inclusiveMaximumName ?? inclusiveMaximum.ToString())}.");
            }
        }

        public static void NotEqualTo<T>(string paramName, [ValidatedNotNull] T value, T unequalValue, string unequalValueName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);

            if (value.CompareTo(unequalValue) == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must not be equal to {(unequalValueName ?? unequalValue.ToString())}.");
            }
        }

        public static void OfType<T>(string paramName, object value)
        {
            OfType(paramName, value, typeof(T));
        }

        public static void OfType(string paramName, object value, Type ofType)
        {
            if (value == null)
            {
                return;
            }

            OfType(paramName, value.GetType(), ofType);
        }

        public static void OfType<T>(string paramName, Type type)
        {
            OfType(paramName, type, typeof(T));
        }

        public static void OfType(string paramName, Type type, Type ofType)
        {
            NotNull(nameof(ofType), ofType);

            if (type == null)
            {
                return;
            }

            if (!ofType.IsAssignableFrom(type))
            {
                if (ofType.IsInterface)
                {
                    throw new ArgumentException($"Parameter {paramName} must implement {ofType.Name}.", paramName);
                }

                throw new ArgumentException($"Parameter {paramName} must be of type {ofType.Name}.", paramName);
            }
        }

        public static void RegexMatch(string paramName, [ValidatedNotNull] string value, string pattern, string formatName = null, RegexOptions options = RegexOptions.Compiled)
        {
            NotNull(paramName, value);

            if (!Regex.IsMatch(value, pattern, options))
            {
                if (formatName != null)
                {
                    throw new ArgumentException($"Parameter {paramName} must match format {formatName}.", paramName);
                }

                throw new ArgumentException($"Parameter {paramName} must match pattern /{pattern}/.", paramName);
            }
        }

        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}

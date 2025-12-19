// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

#if NETSTANDARD2_0
using NotNullAttribute = KodeAid.Diagnostics.CodeAnalysis.ValidatedNotNullAttribute;
#else
using System.Diagnostics.CodeAnalysis;
#endif

namespace KodeAid
{
    [DebuggerStepThrough]
    public static class ArgCheck
    {
        public static void NotNull(string paramName, [NotNull] object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void NotNullOrDefault<T>(string paramName, [NotNull] T value)
        {
            NotNull(paramName, value);

            if (value.Equals(default(T)))
            {
                throw new ArgumentException($"Parameter {paramName} cannot be {default(T)}.", paramName);
            }
        }

        public static void NotNullOrEmpty(string paramName, [NotNull] string value)
        {
            NotNull(paramName, value);

            if (value.Length == 0)
            {
                throw new ArgumentException($"Parameter {paramName} cannot be an empty string.", paramName);
            }
        }

        public static void NotNullOrWhitespace(string paramName, [NotNull] string value)
        {
            NotNullOrEmpty(paramName, value);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Parameter {paramName} cannot be whitespace only.", paramName);
            }
        }

        public static void NotNullOrEmpty<T>(string paramName, [NotNull] ICollection<T> value)
        {
            NotNull(paramName, value);

            if (value.Count == 0)
            {
                throw new ArgumentException($"Parameter {paramName} cannot be empty.", paramName);
            }
        }

        public static void NotNullOrEmpty<T>(string paramName, [NotNull] IEnumerable<T> value)
        {
            NotNull(paramName, value);

            if (!value.Any())
            {
                throw new ArgumentException($"Parameter {paramName} cannot be empty.", paramName);
            }
        }

        public static void NotNullOrEmpty(string paramName, [NotNull] IEnumerable value)
        {
            NotNull(paramName, value);

            if (!value.Cast<object>().Any())
            {
                throw new ArgumentException($"Parameter {paramName} cannot be empty.", paramName);
            }
        }

        public static void NotEqualTo<T>(string paramName, IEquatable<T> value, T unequalValue, string unequalValueName = null)
        {
            if (value is null)
            {
                return;
            }

            if (value.Equals(unequalValue))
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} cannot be {(unequalValueName ?? unequalValue.ToString())}.");
            }
        }

        public static void NotNullOrEqualTo<T>(string paramName, [NotNull] IEquatable<T> value, T unequalValue, string unequalValueName = null)
        {
            NotNull(paramName, value);
            NotEqualTo(paramName, value, unequalValue, unequalValueName);
        }

        public static void GreaterThan(string paramName, IComparable value, object exclusiveMinimum, string exclusiveMinimumName = null)
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(exclusiveMinimum) <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be greater than {(exclusiveMinimumName ?? exclusiveMinimum)}.");
            }
        }

        public static void NotNullAndGreaterThan(string paramName, [NotNull] IComparable value, object exclusiveMinimum, string exclusiveMinimumName = null)
        {
            NotNull(paramName, value);
            GreaterThan(paramName, value, exclusiveMinimum, exclusiveMinimumName);
        }

        public static void GreaterThanOrEqualTo(string paramName, IComparable value, object inclusiveMinimum, string inclusiveMinimumName = null)
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(inclusiveMinimum) < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be greater than or equal to {(inclusiveMinimumName ?? inclusiveMinimum)}.");
            }
        }

        public static void NotNullAndGreaterThanOrEqualTo(string paramName, [NotNull] IComparable value, object inclusiveMinimum, string inclusiveMinimumName = null)
        {
            NotNull(paramName, value);
            GreaterThanOrEqualTo(paramName, value, inclusiveMinimum, inclusiveMinimumName);
        }

        public static void LessThan(string paramName, IComparable value, object exclusiveMaximum, string exclusiveMaximumName = null)
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(exclusiveMaximum) >= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be less than {(exclusiveMaximumName ?? exclusiveMaximum)}.");
            }
        }

        public static void NotNullAndLessThan(string paramName, [NotNull] IComparable value, object exclusiveMaximum, string exclusiveMaximumName = null)
        {
            NotNull(paramName, value);
            LessThan(paramName, value, exclusiveMaximum, exclusiveMaximumName);
        }

        public static void LessThanOrEqualTo(string paramName, IComparable value, object inclusiveMaximum, string inclusiveMaximumName = null)
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(inclusiveMaximum) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be less than or equal to {(inclusiveMaximumName ?? inclusiveMaximum)}.");
            }
        }

        public static void NotNullAnddLessThanOrEqualTo(string paramName, [NotNull] IComparable value, object inclusiveMaximum, string inclusiveMaximumName = null)
        {
            NotNull(paramName, value);
            LessThanOrEqualTo(paramName, value, inclusiveMaximum, inclusiveMaximumName);
        }

        public static void NotEqualTo(string paramName, IComparable value, object unequalValue, string unequalValueName = null)
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(unequalValue) == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must not be equal to {(unequalValueName ?? unequalValue)}.");
            }
        }

        public static void NotNullOrEqualTo(string paramName, [NotNull] IComparable value, object unequalValue, string unequalValueName = null)
        {
            NotNull(paramName, value);
            NotEqualTo(paramName, value, unequalValue, unequalValueName);
        }

        public static void GreaterThan<T>(string paramName, T value, T exclusiveMinimum, string exclusiveMinimumName = null)
            where T : IComparable<T>
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(exclusiveMinimum) <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be greater than {(exclusiveMinimumName ?? exclusiveMinimum.ToString())}.");
            }
        }

        public static void NotNullAndGreaterThan<T>(string paramName, [NotNull] T value, T exclusiveMinimum, string exclusiveMinimumName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);
            GreaterThan(paramName, value, exclusiveMinimum, exclusiveMinimumName);
        }

        public static void GreaterThanOrEqualTo<T>(string paramName, T value, T inclusiveMinimum, string inclusiveMinimumName = null)
            where T : IComparable<T>
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(inclusiveMinimum) < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be greater than or equal to {(inclusiveMinimumName ?? inclusiveMinimum.ToString())}.");
            }
        }

        public static void NotNullAndGreaterThanOrEqualTo<T>(string paramName, [NotNull] T value, T inclusiveMinimum, string inclusiveMinimumName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);
            GreaterThanOrEqualTo(paramName, value, inclusiveMinimum, inclusiveMinimumName);
        }

        public static void LessThan<T>(string paramName, T value, T exclusiveMaximum, string exclusiveMaximumName = null)
            where T : IComparable<T>
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(exclusiveMaximum) >= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be less than {(exclusiveMaximumName ?? exclusiveMaximum.ToString())}.");
            }
        }

        public static void NotNullAndLessThan<T>(string paramName, [NotNull] T value, T exclusiveMaximum, string exclusiveMaximumName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);
            LessThan(paramName, value, exclusiveMaximum, exclusiveMaximumName);
        }

        public static void LessThanOrEqualTo<T>(string paramName, T value, T inclusiveMaximum, string inclusiveMaximumName = null)
            where T : IComparable<T>
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(inclusiveMaximum) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must be less than or equal to {(inclusiveMaximumName ?? inclusiveMaximum.ToString())}.");
            }
        }

        public static void NotNullAndLessThanOrEqualTo<T>(string paramName, [NotNull] T value, T inclusiveMaximum, string inclusiveMaximumName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);
            LessThanOrEqualTo(paramName, value, inclusiveMaximum, inclusiveMaximumName);
        }

        public static void NotEqualTo<T>(string paramName, T value, T unequalValue, string unequalValueName = null)
            where T : IComparable<T>
        {
            if (value is null)
            {
                return;
            }

            if (value.CompareTo(unequalValue) == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"Parameter {paramName} must not be equal to {(unequalValueName ?? unequalValue.ToString())}.");
            }
        }

        public static void NotNullOrEqualTo<T>(string paramName, [NotNull] T value, T unequalValue, string unequalValueName = null)
            where T : IComparable<T>
        {
            NotNull(paramName, value);
            NotEqualTo(paramName, value, unequalValue, unequalValueName);
        }

        public static void RegexMatch(string paramName, string value, string pattern, string formatName = null, RegexOptions options = RegexOptions.Compiled)
        {
            if (value is null)
            {
                return;
            }

            if (!Regex.IsMatch(value, pattern, options))
            {
                if (formatName != null)
                {
                    throw new ArgumentException($"Parameter {paramName} must match format {formatName}.", paramName);
                }

                throw new ArgumentException($"Parameter {paramName} must match pattern /{pattern}/.", paramName);
            }
        }

        public static void NotNullAndRegexMatch(string paramName, [NotNull] string value, string pattern, string formatName = null, RegexOptions options = RegexOptions.Compiled)
        {
            NotNull(paramName, value);
            RegexMatch(paramName, value, pattern, formatName, options);
        }

        public static void OfType<T>(string paramName, object value)
        {
            OfType(paramName, value, typeof(T));
        }

        public static void NotNullAndOfType<T>(string paramName, [NotNull] object value)
        {
            NotNull(paramName, value);
            OfType<T>(paramName, value);
        }

        public static void OfType(string paramName, object value, Type ofType)
        {
            if (value is null)
            {
                return;
            }

            OfType(paramName, value.GetType(), ofType);
        }

        public static void NotNullAndOfType(string paramName, [NotNull] object value, Type ofType)
        {
            NotNull(paramName, value);
            OfType(paramName, value, ofType);
        }

        public static void OfType<T>(string paramName, Type type)
        {
            OfType(paramName, type, typeof(T));
        }

        public static void OfType(string paramName, Type type, Type ofType)
        {
            NotNull(nameof(ofType), ofType);

            if (type is null)
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
    }
}

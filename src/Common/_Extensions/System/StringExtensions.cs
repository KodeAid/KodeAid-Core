// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KodeAid;

namespace System
{
    public static class StringExtensions
    {
        public static string Intern(this string str)
        {
            return str != null ? string.Intern(str) : null;
        }

        public static byte[] ToUtf8Bytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static byte[] FromBase32String(this string base32String)
        {
            return Base32Encoder.DecodeBytes(base32String);
        }

        public static byte[] FromZBase32String(this string zbase32String)
        {
            return ZBase32Encoder.DecodeBytes(zbase32String);
        }

        public static byte[] FromBase64String(this string base64String)
        {
            return Base64Encoder.DecodeBytes(base64String);
        }

        [Obsolete("Use " + nameof(FromBase64String) + "() or " + nameof(FromBase64Url) + "() instead.")]
        public static byte[] FromBase64String(this string base64String, bool urlEncoded)
        {
            if (urlEncoded)
            {
                return base64String.FromBase64Url();
            }

            return base64String.FromBase64String();
        }

        public static byte[] FromBase64Url(this string base64Url)
        {
            return Base64UrlEncoder.DecodeBytes(base64Url);
        }

        public static int GetBase64DecodedByteCount(this string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return 0;
            }

            var characterCount = base64String.Length;
            var paddingCount = base64String.Reverse().TakeWhile(c => c == '=').Count();
            return (3 * (characterCount / 4)) - paddingCount;
        }

        internal static byte[] FromBase36String(this string base36String)
        {
            return Base36Encoder.DecodeBytes(base36String);
        }

        /// <summary>
        /// Trims a string of all leading and trailing whitespace, and returns null if the result would be an empty string.
        /// </summary>
        /// <param name="str">The string to be trimmed.</param>
        /// <returns>A non whitespace-only string; or null if the result would be an empty string.</returns>
        public static string TrimToNull(this string str)
        {
            if (str == null)
            {
                return null;
            }

            str = str.Trim();
            if (str.Length == 0)
            {
                return null;
            }

            return str;
        }

        /// <summary>
        /// Collapse all runs of multiple whitespace into a single space character.
        /// </summary>
        /// <param name="str">The string to collapse.</param>
        /// <param name="multiline">If true, lines will be kept but runs of multiple blank lines will be collasped into a single <paramref name="newLine"/> string</param>
        /// <param name="newLine">The new line string to use for new-lines, if null will default to <seealso cref="Environment.NewLine"/>.</param>
        /// <returns>A string that has all white space collapsed.</returns>
        public static string Collapse(this string str, bool multiline = false, string newLine = null)
        {
            if (multiline)
            {
                return string.Join(newLine ?? Environment.NewLine, str.SplitAndRemoveWhiteSpaceEntries('\r', '\n').Select(line => line.CollapseAndTrim()));
            }

            return Regex.Replace(str, @"\s+", " ");
        }

        /// <summary>
        /// Performs a <see cref="Collapse(string, bool, string)"/> and <see cref="string.Trim"/>.
        /// </summary>
        /// <param name="str">The string to collapse and trim.</param>
        /// <param name="multiline">If true, lines will be kept but runs of multiple blank lines will be collasped into a single <paramref name="newLine"/> string;
        /// all leading and trailing new-lines however will be trimmed.</param>
        /// <param name="newLine">The new line string to use for new-lines, if null will default to <seealso cref="Environment.NewLine"/>.</param>
        /// <returns>A string that has all white space collapsed and all leading and trailing whitespace trimmed.</returns>
        public static string CollapseAndTrim(this string str, bool multiline = false, string newLine = null)
        {
            return Collapse(str, multiline, newLine).Trim();
        }

        /// <summary>
        /// Performs a <see cref="Collapse(string, bool, string)"/> and <see cref="TrimToNull(string)"/>.
        /// </summary>
        /// <param name="str">The string to collapse and trim to null.</param>
        /// <param name="multiline">If true, lines will be kept but runs of multiple blank lines will be collasped into a single <paramref name="newLine"/> string;
        /// all leading and trailing new-lines however will be trimmed.</param>
        /// <param name="newLine">The new line string to use for new-lines, if null will default to <seealso cref="Environment.NewLine"/>.</param>
        /// <returns>A non whitespace-only string that has all white space collapsed and all leading and trailing whitespace trimmed; or null if the result would be an empty string.</returns>
        public static string CollapseAndTrimToNull(this string str, bool multiline = false, string newLine = null)
        {
            return Collapse(str, multiline, newLine).TrimToNull();
        }

        /// <summary>
        /// Truncates a string to a specified maximum length.
        /// </summary>
        /// <param name="str">The string to truncate.</param>
        /// <param name="maxLength">The maximum length of the new string.</param>
        /// <returns>If the length of <paramref name="str"/> is equal to or less than <paramref name="maxLength"/>
        /// then the original <paramref name="str"/> is returned; otherwise the excess trailing characters are removed and
        /// the result will be of length <paramref name="maxLength"/>.</returns>
        public static string Truncate(this string str, int maxLength)
        {
            return str.Length <= maxLength ? str : str.Substring(0, maxLength);
        }

        public static string[] SplitAndRemoveWhiteSpaceEntries(this string str, params char[] separator)
        {
            return str.Split(separator).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
        }

        /// <summary>
        /// Concatenates the members of a constructed <seealso cref="IEnumerable{T}"/> collection of type <seealso cref="string"/>
        /// using the specified <paramref name="separator"/> between each member and escaping any <paramref name="separator"/> characters found in the members.
        /// </summary>
        /// <param name="values">A collection that contains the strings to concatenate.</param>
        /// <param name="separator">The character to use as a separator. It is included in the returned string only if values has more than one element.</param>
        /// <param name="escape">The escape character to escape pre-exisiting <paramref name="separator"/> characters found within <paramref name="values"/>.</param>
        /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns <seealso cref="string.Empty"/>.</returns>
        public static string JoinEscaped(this IEnumerable<string> values, char separator, char escape = '\\')
        {
            if (separator == escape)
            {
                throw new ArgumentException($"Parameter {nameof(separator)} cannot equal the {nameof(escape)} character.", nameof(separator));
            }

            var escapedDelimiter = $"{escape.ToString()}{separator.ToString()}";
            var escapedEscape = $"{escape.ToString()}{escape.ToString()}";
            values = values.Select(v => v.Replace(escape.ToString(), escapedEscape).Replace(separator.ToString(), escapedDelimiter));
            return string.Join(separator.ToString(), values);
        }

        /// <summary>
        /// Splits a string into a maximum number of substrings based on the strings in an array.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator">A char that delimits the substrings in this string.</param>
        /// <param name="escape">The escape character to unescape pre-exisiting <paramref name="separator"/> characters that are not an actual separator.</param>
        /// <returns>An array whose elements contain the substrings from this instance that are delimited by <paramref name="separator"/>.</returns>
        public static string[] SplitEscaped(this string str, char separator, char escape = '\\')
        {
            if (separator == escape)
            {
                throw new ArgumentException($"Parameter {nameof(separator)} cannot equal the {nameof(escape)} character.", nameof(separator));
            }

            var escapedDelimiter = $"{escape.ToString()}{separator.ToString()}";
            var escapedEscape = $"{escape.ToString()}{escape.ToString()}";
            return Regex.Matches(str, $@"(?:[^{Regex.Escape(escape.ToString())}{Regex.Escape(separator.ToString())}]|{Regex.Escape(escapedDelimiter)}|{Regex.Escape(escapedEscape)})+", RegexOptions.Compiled)
              .OfType<Match>()
              .SelectMany(m => m.Captures
                .OfType<Capture>()
                .Select(c => c.Value))
              .Select(v => v.Replace(escapedDelimiter, separator.ToString()).Replace(escapedEscape, escape.ToString()))
              .ToArray();
        }

        /// <summary>
        /// Returns a value indicating whether a specified token occurs within a string.
        /// The characters '}' and ':' are not supported in the token name.
        /// </summary>
        /// <param name="str">Input string that may contain one or more tokens.</param>
        /// <param name="token">Name of the token to seek, eg: use "MyToken" to find "{MyToken:F2}".</param>
        /// <returns>True if the <paramref name="token"/> parameter occurs within <paramref name="str"/>; otherwise, false.</returns>
        public static bool ContainsToken(this string str, string token)
        {
            ArgCheck.NotNull(nameof(str), str);
            ArgCheck.NotNullOrEmpty(nameof(token), token);
            var match = Regex.Match(str, $@"{{{token}(?::([^}}]*))?}}", RegexOptions.Compiled);
            return match.Success && match.Groups[0].Success;
        }

        /// <summary>
        /// Replaces the first matching token in a string, eg: "Insert{MyToken}Here".
        /// The token may contain a format, eg: "{MyToken:F2}" or "{MyToken:yyyy-MM-dd}".
        /// The characters '}' and ':' are not supported in the token name.
        /// You can compare the resulting string to the original string to determine if a replacement took place.
        /// </summary>
        /// <param name="str">Input string that may contain one or more tokens.</param>
        /// <param name="token">Name of the token to replace, eg: use "MyToken" to replace "{MyToken}".</param>
        /// <param name="value">Replacement string value.</param>
        /// <returns>The result of <paramref name="str"/> with the matching token (if found) replaced with <paramref name="value"/>.</returns>
        public static string ReplaceToken(this string str, string token, string value)
        {
            ArgCheck.NotNull(nameof(str), str);
            ArgCheck.NotNullOrEmpty(nameof(token), token);
            var match = Regex.Match(str, $@"{{{token}(?::([^}}]*))?}}", RegexOptions.Compiled);
            if (match.Success && match.Groups[0].Success)
            {
                return str.Replace(match.Groups[0].Value, value);
            }
            return str;
        }

        /// <summary>
        /// Replaces the first matching token in a string, eg: "Insert{MyToken}Here".
        /// The token may contain a format, eg: "{MyToken:F2}" or "{MyToken:yyyy-MM-dd}".
        /// The characters '}' and ':' are not supported in the token name.
        /// You can compare the resulting string to the original string to determine if a replacement took place.
        /// </summary>
        /// <param name="str">Input string that may contain one or more tokens.</param>
        /// <param name="token">Name of the token to replace, eg: use "MyToken" to replace "{MyToken}".</param>
        /// <param name="value">Replacement value, to be converted to a string unformatted.</param>
        /// <returns>The result of <paramref name="str"/> with the matching token (if found) replaced with an
        /// unformatted string representation of <paramref name="value"/>.</returns>
        public static string ReplaceToken(this string str, string token, object value)
        {
            ArgCheck.NotNull(nameof(str), str);
            ArgCheck.NotNullOrEmpty(nameof(token), token);
            var match = Regex.Match(str, $@"{{{token}(?::([^}}]*))?}}", RegexOptions.Compiled);
            if (match.Success && match.Groups[0].Success)
            {
                return str.Replace(match.Groups[0].Value, value?.ToString());
            }
            return str;
        }

        /// <summary>
        /// Replaces the first matching token in a string, eg: "Insert{MyToken}Here".
        /// The token may contain a format, eg: "{MyToken:F2}" or "{MyToken:yyyy-MM-dd}".
        /// The characters '}' and ':' are not supported in the token name.
        /// You can compare the resulting string to the original string to determine if a replacement took place.
        /// </summary>
        /// <param name="str">Input string that may contain one or more tokens.</param>
        /// <param name="token">Name of the token to replace, eg: use "MyToken" to replace "{MyToken}".</param>
        /// <param name="value">Replacement value, to be converted to a string formatted with either the format specified by the token
        /// otherwise by <paramref name="defaultFormat"/>.</param>
        /// <param name="defaultFormat">Default format to use on the <paramref name="value"/> if no format is specified on the matched token.</param>
        /// <param name="formatProvider">The provider to use to format the <paramref name="value"/>. -or-
        /// A null reference to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>The result of <paramref name="str"/> with the matching token (if found) replaced with a formatted <paramref name="value"/>.</returns>
        public static string ReplaceToken(this string str, string token, IFormattable value, string defaultFormat = null, IFormatProvider formatProvider = null)
        {
            ArgCheck.NotNull(nameof(str), str);
            ArgCheck.NotNullOrEmpty(nameof(token), token);
            var match = Regex.Match(str, $@"{{{token}(?::([^}}]*))?}}", RegexOptions.Compiled);
            if (match.Success && match.Groups[0].Success)
            {
                return str.Replace(match.Groups[0].Value,
                    value?.ToString(match.Groups[1].Success ? match.Groups[1].Value : defaultFormat, formatProvider));
            }
            return str;
        }

        public static string Pluralize(this string str)
        {
            if (str == null)
            {
                return null;
            }

            var isUpperCase = char.IsUpper(str, str.Length - 1);

            // ie: butterflies
            if (str.EndsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(isUpperCase ? "{0}IES" : "{0}ies", str.Substring(0, str.Length - 1));
            }

            // ie: dishes
            if (str.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(isUpperCase ? "{0}ES" : "{0}es", str);
            }

            return string.Format(isUpperCase ? "{0}S" : "{0}s", str);
        }

        /// <summary>
        /// Convert from "ResourceNotFound" or "resourceNotFound" to "Resource not found".
        /// </summary>
        public static string ToSentenceCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            str = Regex.Replace(str, "(([a-z][A-Z])|([0-9][A-Za-z])|([a-zA-z][0-9]))", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}", RegexOptions.Compiled);
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Convert from "ResourceNotFound" or "resourceNotFound" to "Resource Not Found".
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            str = Regex.Replace(str, "(([a-z][A-Z])|([0-9][A-Za-z])|([a-zA-z][0-9]))", m => $"{m.Value[0]} {char.ToUpper(m.Value[1])}", RegexOptions.Compiled);
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Convert from "Resource not found" to "ResourceNotFound".
        /// </summary>
        public static string ToPascalCase(this string str)
        {
            if (str == null)
            {
                return str;
            }

            str = Regex.Replace(Regex.Replace(str, "(\\s[a-z])", m => $"{char.ToUpper(m.Value[1])}"), "([^A-Za-z0-9])", "", RegexOptions.Compiled).TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            if (str.Length > 0)
            {
                str = char.ToUpper(str[0]) + str.Substring(1);
            }

            return str;
        }

        /// <summary>
        /// Convert from "Resource not found" to "resourceNotFound".
        /// </summary>
        public static string ToCamelCase(this string str)
        {
            if (str == null)
            {
                return str;
            }

            str = Regex.Replace(Regex.Replace(str, "(\\s[a-z])", m => $"{char.ToUpper(m.Value[1])}"), "([^A-Za-z0-9])", "", RegexOptions.Compiled).TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            if (str.Length > 0)
            {
                str = char.ToLower(str[0]) + str.Substring(1);
            }

            return str;
        }

        /// <summary>
        /// Convert from "ResourceNotFound" or "Resource NOT found!" to "RESOURCE_NOT_FOUND"
        /// </summary>
        public static string ToMacroConstantCase(this string str)
        {
            if (str == null)
            {
                return str;
            }

            // remove unwanted characters (replace with underscore)
            str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
            // if camel casing, then add underscores between words
            str = Regex.Replace(str, "(([a-z][A-Z])|([0-9][A-Za-z])|([a-zA-Z][0-9]))", m => $"{m.Value[0]}_{m.Value[1]}", RegexOptions.Compiled);

            // remove excess underscores
            str = str.Trim('_');
            str = Regex.Replace(str, "[_]{2,}", "_");

            // capitalize full text
            str = str.ToUpperInvariant();
            return str;
        }

        /// <summary>
        /// Remove all diacritics (such as accents, cedilla and other glyphs) from each
        /// character in the string and normalize as Unicode form C.
        /// </summary>
        public static string RemoveDiacritics(this string str)
        {
            ArgCheck.NotNull(nameof(str), str);
            return string.Concat(str
                .Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
                .Normalize(NormalizationForm.FormC);
        }

        public static string EnsureTerminatingPuncuation(this string str, string defaultPuncuation = ".")
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            var endingWhiteSpaceLength = str.Reverse().TakeWhile(char.IsWhiteSpace).Count();
            var endingChar = str[str.Length - (endingWhiteSpaceLength + 1)];

            if (endingChar == '.' || endingChar == '!' || endingChar == '?')
            {
                return str;
            }

            str = str.Insert(str.Length - endingWhiteSpaceLength, defaultPuncuation);

            return str;
        }
    }
}

// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
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

        public static string TrimToNull(this string str)
        {
            if (str == null)
                return null;
            str = str.Trim();
            if (str.Length == 0)
                return null;
            return str;
        }

        public static string Collapse(this string str, bool multiline = false, string newLine = null)
        {
            if (multiline)
                return string.Join(newLine ?? Environment.NewLine, str.SplitAndRemoveWhiteSpaceEntries('\r', '\n').Select(line => line.CollapseAndTrim()));
            return Regex.Replace(str, @"\s+", " ");
        }

        public static string CollapseAndTrim(this string str, bool multiline = false, string newLine = null)
        {
            return Collapse(str, multiline, newLine).Trim();
        }

        public static string CollapseAndTrimToNull(this string str, bool multiline = false, string newLine = null)
        {
            return Collapse(str, multiline, newLine).TrimToNull();
        }

        public static string Truncate(this string str, int maxLength)
        {
            return str.Length <= maxLength ? str : str.Substring(0, maxLength);
        }

        public static string[] SplitAndRemoveWhiteSpaceEntries(this string str, params char[] separator)
        {
            return str.Split(separator).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
        }

        public static byte[] FromBase64(this string base64String, bool urlEncoded = false)
        {
            return Base64Encoder.DecodeBytes(base64String, urlEncoded);
        }

        public static string JoinEscaped(this IEnumerable<string> values, char separator, char escape = '\\')
        {
            var escapedDelimiter = $"{escape.ToString()}{separator.ToString()}";
            var escapedEscape = $"{escape.ToString()}{escape.ToString()}";
            values = values.Select(v => v.Replace(escape.ToString(), escapedEscape).Replace(separator.ToString(), escapedDelimiter));
            return string.Join(separator.ToString(), values);
        }

        public static string[] SplitEscaped(this string str, char separator, char escape = '\\')
        {
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

        public static string Pluralize(this string str)
        {
            if (str == null)
                return null;

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
        /// Convert from "ResourceNotFound" to "Resource not found"
        /// </summary>
        public static string ToSentenceCase(this string str)
        {
            if (str == null)
                return str;
            return Regex.Replace(str, "(([a-z][A-Z])|([0-9][A-Za-z])|([a-zA-z][0-9]))", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}", RegexOptions.Compiled);
        }

        /// <summary>
        /// Convert from "ResourceNotFound" to "Resource Not Found"
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (str == null)
                return str;
            return Regex.Replace(str, "(([a-z][A-Z])|([0-9][A-Za-z])|([a-zA-z][0-9]))", m => $"{m.Value[0]} {char.ToUpper(m.Value[1])}", RegexOptions.Compiled);
        }

        /// <summary>
        /// Convert from "Resource not found" to "ResourceNotFound"
        /// </summary>
        public static string ToPascalCase(this string str)
        {
            if (str == null)
                return str;
            str = Regex.Replace(Regex.Replace(str, "(\\s[a-z])", m => $"{char.ToUpper(m.Value[1])}"), "([^A-Za-z0-9])", "", RegexOptions.Compiled).TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            if (str.Length > 0)
                str = char.ToUpper(str[0]) + str.Substring(1);
            return str;
        }

        /// <summary>
        /// Convert from "Resource not found" to "resourceNotFound"
        /// </summary>
        public static string ToCamelCase(this string str)
        {
            if (str == null)
                return str;
            str = Regex.Replace(Regex.Replace(str, "(\\s[a-z])", m => $"{char.ToUpper(m.Value[1])}"), "([^A-Za-z0-9])", "", RegexOptions.Compiled).TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            if (str.Length > 0)
                str = char.ToLower(str[0]) + str.Substring(1);
            return str;
        }

        /// <summary>
        /// Convert from "ResourceNotFound" or "Resource NOT found!" to "RESOURCE_NOT_FOUND"
        /// </summary>
        public static string ToMacroConstantCase(this string str)
        {
            if (str == null)
                return str;

            // remove unwanted characters (replace with underscore)
            str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
            // if camel casing, then add underscores between words
            str = Regex.Replace(str, "(([a-z][A-Z])|([0-9][A-Za-z])|([a-zA-z][0-9]))", m => $"{m.Value[0]}_{m.Value[1]}", RegexOptions.Compiled);
            // remove excess underscores
            str = str.Trim('_');
            if (str.Contains("__"))
                str = string.Join("_", str.Split('_').Where(s => s.Length > 0));
            // capitalize full text
            str = str.ToUpperInvariant();
            return str;
        }
    }
}

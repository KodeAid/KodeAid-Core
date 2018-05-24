// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KodeAid
{
    public static class Base64Encoder
    {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;

        public static string EncodeString(string value, bool urlEncoded = false)
        {
            if (value == null)
                return null;
            return EncodeBytes(_defaultEncoding.GetBytes(value), urlEncoded);
        }

        public static string EncodeStrings(params string[] values)
        {
            return EncodeStrings(values, false);
        }

        public static string EncodeStrings(bool urlEncoded = false, params string[] values)
        {
            return EncodeStrings(values, urlEncoded);
        }

        public static string EncodeStrings(string[] values, bool urlEncoded = false)
        {
            if (values == null)
                return null;
            return EncodeBytes(GetBytesFromStrings(_defaultEncoding, values), urlEncoded);
        }

        public static string EncodeBytes(byte[] data, bool urlEncoded = false)
        {
            if (data == null)
                return null;

            var base64String = Convert.ToBase64String(data);

            // https://en.wikipedia.org/wiki/Base64#URL_applications
            // https://brockallen.com/2014/10/17/base64url-encoding/
            if (urlEncoded)
                base64String = base64String.Replace("+", "-").Replace("/", "_").TrimEnd('=');

            return base64String;
        }

        public static string DecodeString(string base64String, bool urlEncoded = false)
        {
            if (base64String == null)
                return null;
            return _defaultEncoding.GetString(DecodeBytes(base64String, urlEncoded));
        }

        public static string[] DecodeStrings(string base64String, bool urlEncoded = false)
        {
            if (base64String == null)
                return null;
            return GetStringsFromBytes(_defaultEncoding, DecodeBytes(base64String, urlEncoded));
        }

        public static byte[] DecodeBytes(string base64String, bool urlEncoded = false)
        {
            if (base64String == null)
                return null;

            // https://en.wikipedia.org/wiki/Base64#URL_applications
            // https://brockallen.com/2014/10/17/base64url-encoding/
            if (urlEncoded)
                base64String = base64String?.Replace("-", "+").Replace("_", "/");

            base64String = Regex.Replace(base64String, @"\s", "", RegexOptions.Compiled);
            switch (base64String.Length % 4)  // pad with trailing '='s
            {
                case 0: break;  // no pad chars
                case 2: base64String += "=="; break;  // 2 pad chars
                case 3: base64String += "="; break;  // 1 pad char
                default: throw new FormatException("Illegal base64url string.");
            }

            return Convert.FromBase64String(base64String);
        }

        private static byte[] GetBytesFromStrings(Encoding encoding, string[] values)
        {
            ArgCheck.NotNull(nameof(encoding), encoding);
            ArgCheck.NotNullOrEmpty(nameof(values), values);

            using (var ms = new MemoryStream())
            {
                using (var w = new BinaryWriter(ms, encoding, true))
                {
                    w.Write(values.Length);
                    foreach (var v in values)
                    {
                        w.Write(v != null);
                        if (v != null)
                            w.Write(v);
                    }
                    w.Flush();
                }
                ms.Flush();
                return ms.ToArray();
            }
        }

        private static string[] GetStringsFromBytes(Encoding encoding, byte[] data)
        {
            ArgCheck.NotNull(nameof(encoding), encoding);
            ArgCheck.NotNullOrEmpty(nameof(data), data);

            var values = new List<string>();
            using (var ms = new MemoryStream(data))
            {
                using (var r = new BinaryReader(ms, encoding, true))
                {
                    for (var i = r.ReadInt32(); i > 0; --i)
                    {
                        if (r.ReadBoolean())
                            values.Add(r.ReadString());
                        else
                            values.Add(null);
                    }
                }
            }
            return values.ToArray();
        }
    }
}

// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KodeAid.Security.Cryptography
{
    public static class DataProtectorExtensions
    {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;

        public static byte[] ProtectString(this IDataProtector dataProtector, string unprotectedString, Encoding encoding = null)
        {
            ArgCheck.NotNull(nameof(dataProtector), dataProtector);
            ArgCheck.NotNull(nameof(unprotectedString), unprotectedString);

            return dataProtector.ProtectData((encoding ?? _defaultEncoding).GetBytes(unprotectedString));
        }

        public static byte[] ProtectStrings(this IDataProtector dataProtector, string[] unprotectedStrings, Encoding encoding = null)
        {
            ArgCheck.NotNull(nameof(dataProtector), dataProtector);
            ArgCheck.NotNull(nameof(unprotectedStrings), unprotectedStrings);

            return dataProtector.ProtectData(GetBytesFromStrings(unprotectedStrings, encoding ?? _defaultEncoding));
        }

        public static byte[] ProtectStrings(this IDataProtector dataProtector, params string[] unprotectedStrings)
        {
            return dataProtector.ProtectStrings(unprotectedStrings, null);
        }

        public static byte[] ProtectStrings(this IDataProtector dataProtector, Encoding encoding, params string[] unprotectedStrings)
        {
            return dataProtector.ProtectStrings(unprotectedStrings, encoding);
        }

        public static string UnprotectString(this IDataProtector dataProtector, byte[] protectedData, Encoding encoding = null)
        {
            ArgCheck.NotNull(nameof(dataProtector), dataProtector);
            ArgCheck.NotNull(nameof(protectedData), protectedData);

            return (encoding ?? _defaultEncoding).GetString(dataProtector.UnprotectData(protectedData));
        }

        public static string[] UnprotectStrings(this IDataProtector dataProtector, byte[] protectedData, Encoding encoding = null)
        {
            ArgCheck.NotNull(nameof(dataProtector), dataProtector);
            ArgCheck.NotNull(nameof(protectedData), protectedData);

            return GetStringsFromBytes(dataProtector.UnprotectData(protectedData), encoding ?? _defaultEncoding);
        }

        public static string ProtectDataToBase64(this IDataProtector dataProtector, byte[] unprotectedData, bool urlEncoded = false)
        {
            var bytes = dataProtector.ProtectData(unprotectedData);

            if (urlEncoded)
            {
                return bytes.ToBase64Url();
            }

            return bytes.ToBase64String();
        }

        public static string ProtectStringToBase64(this IDataProtector dataProtector, string unprotectedString, Encoding encoding = null, bool urlEncoded = false)
        {
            var bytes = dataProtector.ProtectString(unprotectedString, encoding);

            if (urlEncoded)
            {
                return bytes.ToBase64Url();
            }

            return bytes.ToBase64String();
        }

        public static string ProtectStringsToBase64(this IDataProtector dataProtector, string[] unprotectedStrings, Encoding encoding = null, bool urlEncoded = false)
        {
            var bytes = dataProtector.ProtectStrings(unprotectedStrings, encoding);

            if (urlEncoded)
            {
                return bytes.ToBase64Url();
            }

            return bytes.ToBase64String();
        }

        public static string ProtectStringsToBase64(this IDataProtector dataProtector, bool urlEncoded = false, params string[] unprotectedStrings)
        {
            var bytes = dataProtector.ProtectStrings(unprotectedStrings, null);

            if (urlEncoded)
            {
                return bytes.ToBase64Url();
            }

            return bytes.ToBase64String();
        }

        public static string ProtectStringsToBase64(this IDataProtector dataProtector, Encoding encoding, params string[] unprotectedStrings)
        {
            return dataProtector.ProtectStrings(unprotectedStrings, encoding).ToBase64String();
        }

        public static string ProtectStringsToBase64(this IDataProtector dataProtector, Encoding encoding, bool urlEncoded, params string[] unprotectedStrings)
        {
            var bytes = dataProtector.ProtectStrings(unprotectedStrings, encoding);

            if (urlEncoded)
            {
                return bytes.ToBase64Url();
            }

            return bytes.ToBase64String();
        }

        public static byte[] UnprotectDataFromBase64(this IDataProtector dataProtector, string protectedBase64, bool urlEncoded = false)
        {
            ArgCheck.NotNull(nameof(protectedBase64), protectedBase64);

            var bytes = urlEncoded ? protectedBase64.FromBase64Url() : protectedBase64.FromBase64String();
            return dataProtector.UnprotectData(bytes);
        }

        public static string UnprotectStringFromBase64(this IDataProtector dataProtector, string protectedBase64, Encoding encoding = null, bool urlEncoded = false)
        {
            ArgCheck.NotNull(nameof(protectedBase64), protectedBase64);

            var bytes = urlEncoded ? protectedBase64.FromBase64Url() : protectedBase64.FromBase64String();
            return dataProtector.UnprotectString(bytes, encoding);
        }

        public static string[] UnprotectStringsFromBase64(this IDataProtector dataProtector, string protectedBase64, Encoding encoding = null, bool urlEncoded = false)
        {
            ArgCheck.NotNull(nameof(protectedBase64), protectedBase64);

            var bytes = urlEncoded ? protectedBase64.FromBase64Url() : protectedBase64.FromBase64String();
            return dataProtector.UnprotectStrings(bytes, encoding);
        }

        private static byte[] GetBytesFromStrings(string[] values, Encoding encoding)
        {
            ArgCheck.NotNull(nameof(values), values);
            ArgCheck.NotNull(nameof(encoding), encoding);

            using var ms = new MemoryStream();
            using (var w = new BinaryWriter(ms, encoding, true))
            {
                w.Write(values.Count());

                foreach (var v in values)
                {
                    w.Write(v != null);

                    if (v != null)
                    {
                        w.Write(v);
                    }
                }

                w.Flush();
            }

            ms.Flush();
            return ms.ToArray();
        }

        private static string[] GetStringsFromBytes(byte[] data, Encoding encoding)
        {
            ArgCheck.NotNullOrEmpty(nameof(data), data);
            ArgCheck.NotNull(nameof(encoding), encoding);

            var values = new List<string>();
            using (var ms = new MemoryStream(data))
            {
                using var r = new BinaryReader(ms, encoding, true);

                for (var i = r.ReadInt32(); i > 0; --i)
                {
                    if (r.ReadBoolean())
                    {
                        values.Add(r.ReadString());
                    }
                    else
                    {
                        values.Add(null);
                    }
                }
            }

            return values.ToArray();
        }
    }
}

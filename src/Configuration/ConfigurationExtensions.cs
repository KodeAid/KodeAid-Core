// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using KodeAid;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IEnumerable<T> GetArrayValues<T>(this IConfiguration configuration)
            where T : new()
        {
            return GetArrayValues<T>(configuration, null);
        }

        public static IEnumerable<T> GetArrayValues<T>(this IConfiguration configuration, string key)
        {
            ArgCheck.NotNull(nameof(configuration), configuration);
            ArgCheck.NotNull(nameof(key), key);

            if (key != null)
            {
                configuration = configuration.GetSection(key);
            }

            return configuration
                .AsEnumerable()
                .Select(p =>
                {
                    var lastIndexOfKeyDelimiter = p.Key.LastIndexOf(ConfigurationPath.KeyDelimiter);
                    var indexer = (lastIndexOfKeyDelimiter > -1) ? p.Key.Substring(lastIndexOfKeyDelimiter + 1) : p.Key;
                    return int.TryParse(indexer, out var i) ? i : -1;
                })
                .Where(i => i >= 0)
                .OrderBy(i => i)
                .Select(i => configuration.GetValue<T>($"{i}"));
        }

        public static IEnumerable<T> GetArray<T>(this IConfiguration configuration)
            where T : new()
        {
            return GetArray<T>(configuration, null);
        }

        public static IEnumerable<T> GetArray<T>(this IConfiguration configuration, string key)
            where T : new()
        {
            var array = new List<T>();
            BindArray<T>(configuration, array, key);
            return array;
        }

        public static void BindArray<T>(this IConfiguration configuration, ICollection<T> array)
            where T : new()
        {
            BindArray<T>(configuration, array, null);
        }

        public static void BindArray<T>(this IConfiguration configuration, ICollection<T> array, string key)
            where T : new()
        {
            ArgCheck.NotNull(nameof(configuration), configuration);
            ArgCheck.NotNull(nameof(array), array);

            if (key != null)
            {
                configuration = configuration.GetSection(key);
            }

            array.AddRange(configuration
                .AsEnumerable()
                .Select(p =>
                {
                    var lastIndexOfKeyDelimiter = p.Key.LastIndexOf(ConfigurationPath.KeyDelimiter);
                    var indexer = (lastIndexOfKeyDelimiter > -1) ? p.Key.Substring(lastIndexOfKeyDelimiter + 1) : p.Key;
                    return int.TryParse(indexer, out var i) ? i : -1;
                })
                .Where(i => i >= 0)
                .OrderBy(i => i)
                .Select(i =>
                {
                    var obj = new T();
                    configuration.Bind($"{i}", obj);
                    return obj;
                }));
        }
    }
}

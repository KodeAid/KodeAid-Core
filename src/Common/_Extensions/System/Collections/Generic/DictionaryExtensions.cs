// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            ArgCheck.NotNull(nameof(dictionary), dictionary);

            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            ArgCheck.NotNull(nameof(dictionary), dictionary);

            if (!dictionary.TryGetValue(key, out var v))
            {
                v = value;
                dictionary.Add(key, v);
            }
            
            return v;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            ArgCheck.NotNull(nameof(dictionary), dictionary);
            ArgCheck.NotNull(nameof(valueFactory), valueFactory);

            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueFactory(key);
                dictionary.Add(key, value);
            }

            return value;
        }
    }
}

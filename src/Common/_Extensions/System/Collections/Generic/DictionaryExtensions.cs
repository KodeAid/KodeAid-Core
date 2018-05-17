// Copyright (c) Kris Penner. All rights reserved.
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
                return value;
            return defaultValue;
        }
    }
}

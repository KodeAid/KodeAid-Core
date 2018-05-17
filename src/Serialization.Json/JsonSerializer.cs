// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Newtonsoft.Json;

namespace KodeAid.Serialization.Json
{
    public static class JsonSerializer
    {
        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None);
        }

        public static T Deserialize<T>(string serializedValue)
        {
            return JsonConvert.DeserializeObject<T>(serializedValue);
        }
    }
}

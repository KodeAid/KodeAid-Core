// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Newtonsoft.Json;

namespace KodeAid.Json
{
    /// <summary>
    /// A mutable version of the JsonSerializer, if you can, use the JsonSerializerImmutable instead.
    /// </summary>
    public class JsonSerializer : JsonSerializerBase
    {
        public JsonSerializerSettings Settings { get; set; }

        protected override Newtonsoft.Json.JsonSerializer GetInnerSerializer()
        {
            var settings = Settings;
            if (settings != null)
            {
                return Newtonsoft.Json.JsonSerializer.Create(settings);
            }

            return Newtonsoft.Json.JsonSerializer.CreateDefault();
        }
    }
}

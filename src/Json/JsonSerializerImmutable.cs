// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Newtonsoft.Json;

namespace KodeAid.Json
{
    /// <summary>
    /// An immutable version of the JsonSerializer for performance improvements.
    /// </summary>
    public class JsonSerializerImmutable : JsonSerializerBase
    {
        private readonly Newtonsoft.Json.JsonSerializer _innerSerializer;

        public JsonSerializerImmutable()
            : this(null)
        {
        }

        public JsonSerializerImmutable(JsonSerializerSettings settings)
        {
            _innerSerializer = settings != null ? Newtonsoft.Json.JsonSerializer.Create(settings) : Newtonsoft.Json.JsonSerializer.CreateDefault();
        }

        protected override Newtonsoft.Json.JsonSerializer GetInnerSerializer() => _innerSerializer;
    }
}

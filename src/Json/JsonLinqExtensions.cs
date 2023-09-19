// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Globalization;
using System.IO;
using System.Linq;

namespace Newtonsoft.Json.Linq
{
    public static class JsonLinqExtensions
    {
        // It is possible to have Type = String and Value = null.
        public static bool IsNull(this JValue token)
            => token == null || token.Value == null || token.Type == JTokenType.Null;

        public static void Collapse(this JToken token, bool removeNullProperties = true, bool removeNullArrayItems = true, bool removeEmptyObjects = true, bool removeEmptyArrays = true)
        {
            if (token is JValue value)
            {
                if (value.IsNull() && value.Parent != null)
                {
                    if (removeNullProperties && value.Parent.Type == JTokenType.Property)
                    {
                        value.Parent.Remove();
                    }
                    else if (removeNullArrayItems && value.Parent.Type == JTokenType.Array)
                    {
                        value.Remove();
                    }
                }

                return;
            }

            if (token is JArray array)
            {
                array.ToList().ForEach(n => Collapse(n, removeNullProperties, removeNullArrayItems, removeEmptyObjects, removeEmptyArrays));

                if (removeEmptyArrays && !array.HasValues && array.Parent != null)
                {
                    if (array.Parent.Type == JTokenType.Property)
                    {
                        array.Parent.Remove();
                    }
                    else if (array.Parent.Type == JTokenType.Array)
                    {
                        array.Remove();
                    }
                }

                return;
            }

            if (token is JProperty property)
            {
                Collapse(property.Value, removeNullProperties, removeNullArrayItems, removeEmptyObjects, removeEmptyArrays);
                return;
            }

            if (token is JObject obj)
            {
                obj.Properties().ToList().ForEach(p => Collapse(p, removeNullProperties, removeNullArrayItems, removeEmptyObjects, removeEmptyArrays));

                if (removeEmptyObjects && !obj.HasValues && obj.Parent != null)
                {
                    if (obj.Parent.Type == JTokenType.Property)
                    {
                        obj.Parent.Remove();
                    }
                    else if (obj.Parent.Type == JTokenType.Array)
                    {
                        obj.Remove();
                    }
                }
            }
        }

        /// <summary>
        /// Formats the JSON with indentation of 4 spaces.
        /// </summary>
        public static string ToFormattedJson(this JToken token, params JsonConverter[] converters)
            => ToFormattedJson(token, 4, converters);

        /// <summary>
        /// Formats the JSON with the specified indentation size.
        /// </summary>
        public static string ToFormattedJson(this JToken token, int indentation, params JsonConverter[] converters)
        {
            using var sw = new StringWriter(CultureInfo.InvariantCulture);
            using var jw = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented,
                IndentChar = ' ',
                Indentation = indentation,
            };
            token.WriteTo(jw, converters);
            jw.Flush();
            sw.Flush();
            return sw.ToString();
        }

        /// <summary>
        /// Generates the JSON in a compact form.
        /// </summary>
        public static string ToCompactJson(this JToken token, params JsonConverter[] converters)
        {
            using var sw = new StringWriter(CultureInfo.InvariantCulture);
            using var jw = new JsonTextWriter(sw)
            {
                Formatting = Formatting.None,
            };
            token.WriteTo(jw, converters);
            jw.Flush();
            sw.Flush();
            return sw.ToString();
        }
    }
}

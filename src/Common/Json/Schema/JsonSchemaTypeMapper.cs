// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid.Json.Schema
{
    public static class JsonSchemaTypeMapper
    {
        public static Type GetType(string jsonSchemaType)
        {
            // https://spacetelescope.github.io/understanding-json-schema/reference/type.html
            // https://spacetelescope.github.io/understanding-json-schema/reference/numeric.html
            switch (jsonSchemaType?.ToLower())
            {
                case "array":   // javascript
                case "list":    // python
                    return typeof(object[]);
                case "bool":    // javascript
                case "boolean": // python
                    return typeof(bool);
                case "null":    // javascript
                case "none":    // python
                    return null;
                case "object":  // javascript
                case "dict":    // python
                    return typeof(object);
                case "number":  // javascript
                case "float":   // python
                    return typeof(decimal);
                case "integer": // javascript
                case "int":     // python
                    return typeof(int);
                default:
                    return typeof(string);
            }
        }

        public static string FromXmlSchema(string xmlSchemaType, out string format)
        {
            format = null;
            // http://json-schema.org/latest/json-schema-validation.html#rfc.section.7
            switch (xmlSchemaType?.ToLower())
            {
                case "anyuri":
                    format = "uri";
                    return "string";
                case "base64binary":
                    return "string";
                case "boolean":
                    return "boolean";
                case "byte":
                    return "integer";
                case "date":
                    format = "date";
                    return "string";
                case "datetime":
                    format = "date-time";
                    return "string";
                case "decimal":
                    return "number";
                case "double":
                    return "number";
                case "duration":
                    return "string";
                case "entities":
                    //itemType = "object";
                    return "array";
                case "entity":
                    return "object";
                case "float":
                    return "number";
                case "gday":
                    return "string";
                case "gmonthday":
                    return "string";
                case "gyear":
                    return "string";
                case "gyearmonth":
                    return "string";
                case "hexbinary":
                    return "string";
                case "id":
                    return "string";
                case "idref":
                    return "string";
                case "idrefs":
                    //itemType = "string";
                    return "array";
                case "int":
                    return "integer";
                case "integer":
                    return "string";
                case "language":
                    return "string";
                case "long":
                    return "integer";
                case "month":
                    return "string";
                case "name":
                    return "string";
                case "ncname":
                    return "string";
                case "negativeinteger":
                    return "integer";
                case "nmtoken":
                    return "string";
                case "nmtokens":
                    //itemType = "string";
                    return "array";
                case "nonnegativeinteger":
                    return "integer";
                case "nonpositiveinteger":
                    return "integer";
                case "normalizedstring":
                    return "string";
                case "notation":
                    return "string";
                case "positiveinteger":
                    return "integer";
                case "qname":
                    return "string";
                case "short":
                    return "integer";
                case "string":
                    return "string";
                case "time":
                    format = "time";
                    return "string";
                case "timeperiod":
                    return "string";
                case "token":
                    return "string";
                case "unsignedbyte":
                    return "integer";
                case "unsignedint":
                    return "integer";
                case "unsignedlong":
                    return "integer";
                case "unsignedshort":
                    return "integer";
                default:
                    return "string";
            }
        }
    }
}

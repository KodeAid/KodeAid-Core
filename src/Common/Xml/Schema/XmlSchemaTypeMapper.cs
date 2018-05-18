// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Xml;

namespace KodeAid.Xml.Schema
{
    public static class XmlSchemaTypeMapper
    {
        public static Type GetType(string xmlSchemaType)
        {
            // https://msdn.microsoft.com/en-us/library/aa719879(v=vs.71).aspx
            switch (xmlSchemaType?.ToLower())
            {
                case "anyuri":
                    return typeof(Uri);
                case "base64binary":
                    return typeof(byte[]);  // string??
                case "boolean":
                    return typeof(bool);
                case "byte":
                    return typeof(sbyte);
                case "date":
                    return typeof(DateTime);
                case "datetime":
                    return typeof(DateTime);
                case "decimal":
                    return typeof(decimal);
                case "double":
                    return typeof(double);
                case "duration":
                    return typeof(TimeSpan);
                case "entities":
                    return typeof(string[]);
                case "entity":
                    return typeof(string);
                case "float":
                    return typeof(float);
                case "gday":
                    return typeof(DateTime);
                case "gmonthday":
                    return typeof(DateTime);
                case "gyear":
                    return typeof(DateTime);
                case "gyearmonth":
                    return typeof(DateTime);
                case "hexbinary":
                    return typeof(byte[]);
                case "id":
                    return typeof(string);
                case "idref":
                    return typeof(string);
                case "idrefs":
                    return typeof(string[]);
                case "int":
                    return typeof(int);
                case "integer":
                    return typeof(long);
                case "language":
                    return typeof(string);
                case "long":
                    return typeof(long);
                case "month":
                    return typeof(DateTime);
                case "name":
                    return typeof(string);
                case "ncname":
                    return typeof(string);
                case "negativeinteger":
                    return typeof(long);
                case "nmtoken":
                    return typeof(string);
                case "nmtokens":
                    return typeof(string[]);
                case "nonnegativeinteger":
                    return typeof(long);
                case "nonpositiveinteger":
                    return typeof(long);
                case "normalizedstring":
                    return typeof(string);
                case "notation":
                    return typeof(string);
                case "positiveinteger":
                    return typeof(long);
                case "qname":
                    return typeof(XmlQualifiedName);
                case "short":
                    return typeof(short);
                case "string":
                    return typeof(string);
                case "time":
                    return typeof(DateTime);
                case "timeperiod":
                    return typeof(DateTime);
                case "token":
                    return typeof(string);
                case "unsignedbyte":
                    return typeof(byte);
                case "unsignedint":
                    return typeof(uint);
                case "unsignedlong":
                    return typeof(ulong);
                case "unsignedshort":
                    return typeof(ushort);
                default:
                    return typeof(string);
            }
        }
    }
}

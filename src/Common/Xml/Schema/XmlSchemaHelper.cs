// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Xml;

namespace KodeAid.Xml.Schema
{
    public static class XmlSchemaHelper
    {
        public static Type GetTypeFromSchema(string schemaType)
        {
            ArgCheck.NotNullOrEmpty(nameof(schemaType), schemaType);

            if (!TryGetTypeFromSchema(schemaType, out var type))
            {
                throw new ArgumentException($"Unknown XML schema type: {schemaType}.", nameof(schemaType));
            }

            return type;
        }

        public static Type GetTypeOrDefaultFromSchema(string schemaType, Type defaultType = null)
        {
            return TryGetTypeFromSchema(schemaType, out var type) ? type : defaultType;
        }

        public static bool TryGetTypeFromSchema(string schemaType, out Type type)
        {
            // https://msdn.microsoft.com/en-us/library/aa719879(v=vs.71).aspx
            switch (schemaType?.ToLower())
            {
                case XmlSchemaTypes.AnyUri:
                    type = typeof(Uri);
                    return true;
                case XmlSchemaTypes.Base64Binary:
                    type = typeof(byte[]);  // string??
                    return true;
                case XmlSchemaTypes.Boolean:
                    type = typeof(bool);
                    return true;
                case XmlSchemaTypes.Byte:
                    type = typeof(sbyte);
                    return true;
                case XmlSchemaTypes.Date:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.Datetime:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.Decimal:
                    type = typeof(decimal);
                    return true;
                case XmlSchemaTypes.Double:
                    type = typeof(double);
                    return true;
                case XmlSchemaTypes.Duration:
                    type = typeof(TimeSpan);
                    return true;
                case XmlSchemaTypes.Entities:
                    type = typeof(string[]);
                    return true;
                case XmlSchemaTypes.Entity:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.Float:
                    type = typeof(float);
                    return true;
                case XmlSchemaTypes.Gday:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.GMonthDay:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.GYear:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.GYearMonth:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.HexBinary:
                    type = typeof(byte[]);
                    return true;
                case XmlSchemaTypes.Id:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.IdRef:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.IdRefs:
                    type = typeof(string[]);
                    return true;
                case XmlSchemaTypes.Int:
                    type = typeof(int);
                    return true;
                case XmlSchemaTypes.Integer:
                    type = typeof(long);
                    return true;
                case XmlSchemaTypes.Language:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.Long:
                    type = typeof(long);
                    return true;
                case XmlSchemaTypes.Month:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.Name:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.NcName:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.NegativeInteger:
                    type = typeof(long);
                    return true;
                case XmlSchemaTypes.NmToken:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.NmTokens:
                    type = typeof(string[]);
                    return true;
                case XmlSchemaTypes.NonNegativeInteger:
                    type = typeof(long);
                    return true;
                case XmlSchemaTypes.NonPositiveInteger:
                    type = typeof(long);
                    return true;
                case XmlSchemaTypes.NormalizedString:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.Notation:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.PositiveInteger:
                    type = typeof(long);
                    return true;
                case XmlSchemaTypes.QName:
                    type = typeof(XmlQualifiedName);
                    return true;
                case XmlSchemaTypes.Short:
                    type = typeof(short);
                    return true;
                case XmlSchemaTypes.String:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.Time:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.TimePeriod:
                    type = typeof(DateTime);
                    return true;
                case XmlSchemaTypes.Token:
                    type = typeof(string);
                    return true;
                case XmlSchemaTypes.UnsignedByte:
                    type = typeof(byte);
                    return true;
                case XmlSchemaTypes.UnsignedInt:
                    type = typeof(uint);
                    return true;
                case XmlSchemaTypes.UnsignedLong:
                    type = typeof(ulong);
                    return true;
                case XmlSchemaTypes.UnsignedShort:
                    type = typeof(ushort);
                    return true;
                default:
                    type = null;
                    return false;
            }
        }
    }
}

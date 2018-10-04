// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;

namespace System.Xml.Serialization
{
    public static class XmlSerializerExtensions
    {
        public static void Serialize<T>(this XmlSerializer serializer, Stream stream, T o, bool omitStandardNamespaces)
        {
            serializer.Serialize(stream, o, null, omitStandardNamespaces);
        }

        public static void Serialize<T>(this XmlSerializer serializer, Stream stream, T o, XmlSerializerNamespaces namespaces, bool omitStandardNamespaces)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(stream, o, namespaces);
        }

        public static void Serialize<T>(this XmlSerializer serializer, TextWriter textWriter, T o, bool omitStandardNamespaces)
        {
            serializer.Serialize(textWriter, o, null, omitStandardNamespaces);
        }

        public static void Serialize<T>(this XmlSerializer serializer, TextWriter textWriter, T o, XmlSerializerNamespaces namespaces, bool omitStandardNamespaces)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(textWriter, o, namespaces);
        }

        public static void Serialize<T>(this XmlSerializer serializer, XmlWriter xmlWriter, T o, bool omitStandardNamespaces)
        {
            serializer.Serialize(xmlWriter, o, null, omitStandardNamespaces);
        }

        public static void Serialize<T>(this XmlSerializer serializer, XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces, bool omitStandardNamespaces)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(xmlWriter, o, namespaces);
        }

        public static void Serialize<T>(this XmlSerializer serializer, XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces, bool omitStandardNamespaces, string encodingStyle)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(xmlWriter, o, namespaces, encodingStyle);
        }

        public static void Serialize<T>(this XmlSerializer serializer, XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces, bool omitStandardNamespaces, string encodingStyle, string id)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(xmlWriter, o, namespaces, encodingStyle, id);
        }
    }
}

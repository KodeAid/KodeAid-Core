// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Text;
using KodeAid.Xml;

namespace System.Xml.Serialization
{
    public static class XmlSerializerExtensions
    {
        public static void Serialize(this XmlSerializer serializer, Stream stream, object o, bool omitStandardNamespaces)
        {
            serializer.Serialize(stream, o, null, omitStandardNamespaces);
        }

        public static void Serialize(this XmlSerializer serializer, Stream stream, object o, XmlSerializerNamespaces namespaces, bool omitStandardNamespaces)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(stream, o, namespaces);
        }

        public static void Serialize(this XmlSerializer serializer, TextWriter textWriter, object o, bool omitStandardNamespaces)
        {
            serializer.Serialize(textWriter, o, null, omitStandardNamespaces);
        }

        public static void Serialize(this XmlSerializer serializer, TextWriter textWriter, object o, XmlSerializerNamespaces namespaces, bool omitStandardNamespaces)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(textWriter, o, namespaces);
        }

        public static void Serialize(this XmlSerializer serializer, XmlWriter xmlWriter, object o, bool omitStandardNamespaces)
        {
            serializer.Serialize(xmlWriter, o, null, omitStandardNamespaces);
        }

        public static void Serialize(this XmlSerializer serializer, XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, bool omitStandardNamespaces)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(xmlWriter, o, namespaces);
        }

        public static void Serialize(this XmlSerializer serializer, XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, bool omitStandardNamespaces)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(xmlWriter, o, namespaces, encodingStyle);
        }

        public static void Serialize(this XmlSerializer serializer, XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id, bool omitStandardNamespaces)
        {
            if (omitStandardNamespaces)
            {
                namespaces = namespaces ?? new XmlSerializerNamespaces();
                namespaces.Add("", "");
            }
            serializer.Serialize(xmlWriter, o, namespaces, encodingStyle, id);
        }

        public static string Serialize(this XmlSerializer serializer, object o)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                serializer.Serialize(writer, o);
                writer.Flush();
            }
            return sb.ToString();
        }

        public static string Serialize(this XmlSerializer serializer, object o, XmlSerializerNamespaces namespaces)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                serializer.Serialize(writer, o, namespaces);
                writer.Flush();
            }
            return sb.ToString();
        }

        public static string Serialize(this XmlSerializer serializer, object o, XmlSerializerNamespaces namespaces, string encodingStyle)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                serializer.Serialize(writer, o, namespaces, encodingStyle);
                writer.Flush();
            }
            return sb.ToString();
        }

        public static string Serialize(this XmlSerializer serializer, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                serializer.Serialize(writer, o, namespaces, encodingStyle, id);
                writer.Flush();
            }
            return sb.ToString();
        }

        public static string Serialize(this XmlSerializer serializer, object o, XmlFormatOptions options)
        {
            return XmlHelper.ReformatXml(serializer.Serialize(o), options);
        }

        public static string Serialize(this XmlSerializer serializer, object o, XmlSerializerNamespaces namespaces, XmlFormatOptions options)
        {
            return XmlHelper.ReformatXml(serializer.Serialize(o, namespaces), options);
        }

        public static string Serialize(this XmlSerializer serializer, object o, XmlSerializerNamespaces namespaces, string encodingStyle, XmlFormatOptions options)
        {
            return XmlHelper.ReformatXml(serializer.Serialize(o, namespaces, encodingStyle), options);
        }

        public static string Serialize(this XmlSerializer serializer, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id, XmlFormatOptions options)
        {
            return XmlHelper.ReformatXml(serializer.Serialize(o, namespaces, encodingStyle, id), options);
        }

        public static object Deserialize(this XmlSerializer serializer, string xml, bool ignoreNamespaces = false)
        {
            using (var stream = new StringReader(xml))
            using (var reader = CreateReader(stream, ignoreNamespaces))
            {
                return serializer.Deserialize(reader);
            }
        }

        public static object Deserialize(this XmlSerializer serializer, string xml, string encodingStyle, bool ignoreNamespaces = false)
        {
            using (var stream = new StringReader(xml))
            using (var reader = CreateReader(stream, ignoreNamespaces))
            {
                return serializer.Deserialize(reader, encodingStyle);
            }
        }

        public static object Deserialize(this XmlSerializer serializer, string xml, string encodingStyle, XmlDeserializationEvents events, bool ignoreNamespaces = false)
        {
            using (var stream = new StringReader(xml))
            using (var reader = CreateReader(stream, ignoreNamespaces))
            {
                return serializer.Deserialize(reader, encodingStyle, events);
            }
        }

        private static XmlReader CreateReader(TextReader stream, bool ignoreNamespaces)
        {
            var reader = XmlReader.Create(stream);
            if (ignoreNamespaces)
            {
                reader = new XmlIgnoreNamespaceReader(reader, true);
            }
            return reader;
        }
    }
}

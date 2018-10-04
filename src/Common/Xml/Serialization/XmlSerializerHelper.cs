// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Text;
using System.Xml;
using System.Xml.Serialization;
using KodeAid.IO;

namespace KodeAid.Xml.Serialization
{
    public static class XmlSerializerHelper
    {
        public static string Serialize<T>(T obj, bool indent = false, bool omitStandardNamespaces = false, string defaultNamespace = null, bool omitXmlDeclaration = false, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = indent,
                OmitXmlDeclaration = omitXmlDeclaration,
            };

            var namespaces = new XmlSerializerNamespaces();
            if (omitStandardNamespaces)
            {
                namespaces.Add("", "");
            }
            if (defaultNamespace != null)
            {
                namespaces.Add("", defaultNamespace);
            }

            var sb = new StringBuilder();
            using (var sw = new StringWriter(encoding, sb))
            using (var xw = XmlWriter.Create(sw, settings))
            {
                new XmlSerializer(typeof(T), defaultNamespace).Serialize(xw, obj, namespaces);
                xw.Flush();
                sw.Flush();
            }
            return sb.ToString();
        }
    }
}

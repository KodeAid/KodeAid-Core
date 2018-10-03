// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KodeAid.IO;

namespace KodeAid.Xml
{
    public static class XmlHelper
    {
        public static string ReformatXml(string xml, XmlFormatOptions options)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return xml;
            }

            if (options == null)
            {
                options = new XmlFormatOptions();
            }

            var doc = XDocument.Parse(xml);

            if (options.RemoveAllNamespaces)
            {
                RemoveNamespaces(doc.Root);
            }
            else if (options.OmitStandardNamespaces)
            {
                RemoveStandardNamespaces(doc.Root);
            }

            if (!string.IsNullOrEmpty(options.DefaultNamespace))
            {
                doc.Root.SetDefaultXmlNamespace(options.DefaultNamespace);
            }

            var settings = new XmlWriterSettings()
            {
                Encoding = options.Encoding,
                Indent = options.Indent,
                IndentChars = options.IndentChars,
                NamespaceHandling = options.OmitDuplicateNamespaces ? NamespaceHandling.OmitDuplicates : NamespaceHandling.Default,
                NewLineChars = options.NewLineChars,
                OmitXmlDeclaration = options.OmitXmlDeclaration,
                NewLineOnAttributes = options.NewLineOnAttributes,
                NewLineHandling = options.PreserveNewLines ? NewLineHandling.None : NewLineHandling.Replace,
            };

            var sb = new StringBuilder();
            using (var sw = new StringWriter(options.Encoding, sb))
            using (var xw = XmlWriter.Create(sw, settings))
            {
                doc.WriteTo(xw);
                xw.Flush();
                sw.Flush();
            }

            return sb.ToString();
        }

        private static void RemoveStandardNamespaces(XElement element)
        {
            var attributes = element.Attributes().ToList();
            element.RemoveAttributes();
            foreach (var attr in attributes)
            {
                if (!attr.IsNamespaceDeclaration || (attr.Name.LocalName != "xsi" && attr.Name.LocalName != "xsd"))
                {
                    element.Add(attr.Name.Namespace != null ? new XAttribute(attr.Name.LocalName, attr.Value) : attr);
                }
            };
        }

        private static void RemoveNamespaces(XElement element)
        {
            // remove from element
            if (element.Name.Namespace != null)
            {
                element.Name = element.Name.LocalName;
            }

            // remove from attributes
            var attributes = element.Attributes().ToList();
            element.RemoveAttributes();
            foreach (var attr in attributes)
            {
                if (!attr.IsNamespaceDeclaration)
                {
                    element.Add(attr.Name.Namespace != null ? new XAttribute(attr.Name.LocalName, attr.Value) : attr);
                }
            };

            // remove from children
            foreach (var child in element.Descendants())
            {
                RemoveNamespaces(child);
            }
        }
    }
}

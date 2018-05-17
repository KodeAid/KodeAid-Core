// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System.Xml.Linq
{
    public static class LinqXmlExtensions
    {
        public static string GetChildElementValue(this XElement element, XName childElementName)
        {
            return GetChildElementValue<string>(element, childElementName);
        }

        public static T GetChildElementValue<T>(this XElement element, XName childElementName, T defaultValue = default)
        {
            return (T)GetChildElementValue(element, childElementName, typeof(T), defaultValue);
        }

        public static object GetChildElementValue(this XElement element, XName childElementName, Type type, object defaultValue = null)
        {
            ArgCheck.NotNull(nameof(type), type);
            try
            {
                if (element != null && !string.IsNullOrEmpty(childElementName.LocalName))
                {
                    element = element.Element(childElementName);
                    if (element != null)
                    {
                        return ParseValue(element.Value, type, defaultValue);
                    }
                }
            }
            catch { }
            return ParseValue(null, type, defaultValue);
        }

        public static string GetAttributeValue(this XElement element, XName attributeName)
        {
            return GetAttributeValue(element, attributeName, default(string));
        }

        public static T GetAttributeValue<T>(this XElement element, XName attributeName, T defaultValue = default)
        {
            return (T)GetAttributeValue(element, attributeName, typeof(T), defaultValue);
        }

        public static object GetAttributeValue(this XElement element, XName attributeName, Type type, object defaultValue = null)
        {
            ArgCheck.NotNull(nameof(type), type);
            try
            {
                if (element != null && !string.IsNullOrEmpty(attributeName.LocalName))
                {
                    var attribute = element.Attribute(attributeName);
                    if (attribute != null)
                    {
                        return ParseValue(attribute.Value, type, defaultValue);
                    }
                }
            }
            catch { }
            return ParseValue(null, type, defaultValue);
        }

        public static XElement ToXElement(this XmlNode node)
        {
            if (node == null)
            {
                return null;
            }

            var doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                node.WriteTo(writer);
            }

            return doc.Root;
        }

        public static XmlDocument ToXmlDocument(this XElement element)
        {
            if (element == null)
            {
                return null;
            }

            using (var reader = element.CreateReader())
            {
                var doc = new XmlDocument(); ;
                doc.Load(reader);
                return doc;
            }
        }

        public static XmlElement ToXmlElement(this XElement element)
        {
            if (element == null)
            {
                return null;
            }

            return ToXmlDocument(element).DocumentElement;
        }

        private static object ParseValue(string value, Type type, object defaultValue = null)
        {
            return ParseHelper.ParseOrDefault(value, type, defaultValue);
        }
    }
}

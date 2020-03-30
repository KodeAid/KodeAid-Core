// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using KodeAid;

namespace System.Xml
{
    public static class XmlDocumentExtensions
    {
        public static XmlElement CreateElementFromXml(this XmlDocument doc, string elementXml)
        {
            ArgCheck.NotNull(nameof(doc), doc);
            ArgCheck.NotNullOrEmpty(nameof(elementXml), elementXml);
            var t = doc.CreateElement("T");
            t.InnerXml = elementXml.Trim();
            var e = (XmlElement)t.FirstChild;
            t.RemoveChild(e);
            return e;
        }
    }
}

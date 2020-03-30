// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Text;
using System.Xml;
using KodeAid.Xml;
using Microsoft.AspNetCore.Mvc;

namespace KodeAid.AspNetCore.Mvc.Formatters
{
    public class XmlSerializerInputFormatter : Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerInputFormatter
    {
        private readonly bool _ignoreNamespaces;

        public XmlSerializerInputFormatter(XmlSerializerFormatterOptions options, MvcOptions mvcOptions)
            : base(mvcOptions)
        {
            _ignoreNamespaces = (options?.IgnoreNamespaces).GetValueOrDefault();
        }

        protected override XmlReader CreateXmlReader(Stream readStream, Encoding encoding)
        {
            var reader = base.CreateXmlReader(readStream, encoding);
            if (_ignoreNamespaces)
            {
                reader = new XmlIgnoreNamespaceReader(reader, true);
            }
            return reader;
        }
    }
}

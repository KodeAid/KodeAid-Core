// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.IO;
using System.Text;
using System.Xml;

namespace KodeAid.AspNetCore.Mvc.Formatters
{
    public class XmlSerializerInputFormatter : Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerInputFormatter
    {
        private readonly bool _ignoreNamespaces;

        public XmlSerializerInputFormatter()
            : base()
        {
        }

        public XmlSerializerInputFormatter(bool ignoreNamespaces = false, bool suppressInputFormatterBuffering = false)
            : base(suppressInputFormatterBuffering)
        {
            _ignoreNamespaces = ignoreNamespaces;
        }

        protected override XmlReader CreateXmlReader(Stream readStream, Encoding encoding)
        {
            var reader = base.CreateXmlReader(readStream, encoding) as XmlTextReader;
            if (_ignoreNamespaces)
            {
                reader.Namespaces = false;
            }
            return reader;
        }
    }
}

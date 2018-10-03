// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Text;

namespace KodeAid.Xml
{
    public class XmlFormatOptions
    {
        private Encoding _encoding;

        public string DefaultNamespace { get; set; } = null;
        public Encoding Encoding
        {
            get => _encoding ?? Encoding.UTF8;
            set => _encoding = value;
        }
        public bool Indent { get; set; } = false;
        public string IndentChars { get; set; } = "    ";
        public string NewLineChars { get; set; } = Environment.NewLine;
        public bool NewLineOnAttributes { get; set; } = false;
        public bool OmitDuplicateNamespaces { get; set; } = false;
        public bool OmitStandardNamespaces { get; set; } = false;
        public bool OmitXmlDeclaration { get; set; } = false;
        public bool PreserveNewLines { get; set; } = false;
        public bool RemoveAllNamespaces { get; set; } = false;
    }
}

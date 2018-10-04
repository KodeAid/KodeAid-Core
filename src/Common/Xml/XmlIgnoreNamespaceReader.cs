// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System.Xml;

namespace KodeAid.Xml
{
    public class XmlIgnoreNamespaceReader : XmlReader
    {
        private readonly XmlReader _reader;

        public XmlIgnoreNamespaceReader(XmlReader reader)
        {
            ArgCheck.NotNull(nameof(reader), reader);
            _reader = reader;
        }

        public override int AttributeCount => _reader.AttributeCount;

        public override string BaseURI => _reader.BaseURI;

        public override int Depth => _reader.Depth;

        public override bool EOF => _reader.EOF;

        public override bool IsEmptyElement => _reader.IsEmptyElement;

        public override string LocalName => _reader.LocalName;

        public override string NamespaceURI => string.Empty;

        public override XmlNameTable NameTable => _reader.NameTable;

        public override XmlNodeType NodeType => _reader.NodeType;

        public override string Prefix => _reader.Prefix;

        public override ReadState ReadState => _reader.ReadState;

        public override string Value => _reader.Value;

        public override string GetAttribute(int i)
        {
            return _reader.GetAttribute(i);
        }

        public override string GetAttribute(string name)
        {
            return _reader.GetAttribute(name);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return _reader.GetAttribute(name, namespaceURI);
        }

        public override string LookupNamespace(string prefix)
        {
            return _reader.LookupNamespace(prefix);
        }

        public override bool MoveToAttribute(string name)
        {
            return _reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return _reader.MoveToAttribute(name, ns);
        }

        public override bool MoveToElement()
        {
            return _reader.MoveToElement();
        }

        public override bool MoveToFirstAttribute()
        {
            return _reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return _reader.MoveToNextAttribute();
        }

        public override bool Read()
        {
            return _reader.Read();
        }

        public override bool ReadAttributeValue()
        {
            return _reader.ReadAttributeValue();
        }

        public override void ResolveEntity()
        {
            _reader.ResolveEntity();
        }
    }
}

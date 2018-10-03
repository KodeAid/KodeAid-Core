// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Text;

namespace KodeAid.IO
{
    public class StringWriter : System.IO.StringWriter
    {
        private readonly Encoding _encoding;

        public StringWriter(Encoding encoding)
        {
            _encoding = encoding;
        }

        public StringWriter(Encoding encoding, StringBuilder sb)
            : base(sb)
        {
            _encoding = encoding;
        }

        public StringWriter(Encoding encoding, IFormatProvider formatProvider)
            : base(formatProvider)
        {
            _encoding = encoding;
        }

        public StringWriter(Encoding encoding, StringBuilder sb, IFormatProvider formatProvider)
            : base(sb, formatProvider)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding => _encoding ?? base.Encoding;

        public static StringWriter CreateUtf8Writer()
        {
            return new StringWriter(Encoding.UTF8);
        }

        public static StringWriter CreateUtf8Writer(StringBuilder sb)
        {
            return new StringWriter(Encoding.UTF8, sb);
        }

        public static StringWriter CreateUtf8Writer(IFormatProvider formatProvider)
        {
            return new StringWriter(Encoding.UTF8, formatProvider);
        }

        public static StringWriter CreateUtf8Writer(StringBuilder sb, IFormatProvider formatProvider)
        {
            return new StringWriter(Encoding.UTF8, sb, formatProvider);
        }
    }
}

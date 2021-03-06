// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using HtmlAgilityPack;

namespace KodeAid.Html
{
    public static class HtmlHelper
    {
        public static string GetInnerText(string html, bool multiline = false, string newLine = null, params string[] lineBreakTags)
        {
            if (html == null)
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.GetInnerText(multiline, newLine, lineBreakTags);
        }
    }
}

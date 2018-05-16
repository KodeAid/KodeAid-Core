// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Xml.XPath;

namespace HtmlAgilityPack
{
    public static class HtmlNodeExtensions
    {
        private static readonly string _newLineToken = "{{NEWLINE_" + Guid.NewGuid().ToString("N") + "}}";
        private static readonly string[] _defaultLineBreakTags = new[] { "br", "p" };

        /// <summary>
        /// Selects a list of nodes matching the HtmlAgilityPack.HtmlNode.XPath expression - will not return null.
        /// </summary>
        /// <param name="xpath">The XPath expression.</param>
        /// <returns>An HtmlAgilityPack.HtmlNodeCollection containing a collection of nodes matching the HtmlAgilityPack.HtmlNode.XPath query, or an empty collection if no node matched the XPath expression.</returns>
        public static HtmlNodeCollection SelectSafeNodes(this HtmlNode node, string xpath)
        {
            return node.SelectNodes(xpath) ?? new HtmlNodeCollection(node);
        }

        /// <summary>
        /// Selects a list of nodes matching the HtmlAgilityPack.HtmlNode.XPath expression - will not return null.
        /// </summary>
        /// <param name="xpath">The XPath expression.</param>
        /// <returns>An HtmlAgilityPack.HtmlNodeCollection containing a collection of nodes matching the HtmlAgilityPack.HtmlNode.XPath query, or an empty collection if no node matched the XPath expression.</returns>
        public static HtmlNodeCollection SelectSafeNodes(this HtmlNode node, XPathExpression xpath)
        {
            return node.SelectNodes(xpath) ?? new HtmlNodeCollection(node);
        }

        /// <summary>
        /// Gets the unescaped inner text of a node, optionally with new lines inserted for line-breaking elements such as "br" and "p".
        /// </summary>
        /// <param name="multiline">True to replace line-breaking elements with new lines; otherwise false to collapse to a single line.</param>
        /// <param name="newLine">The new line string to use, defaults to <see cref="System.Environment.NewLine"/>, only applicable if <paramref name="multiline"/> is true.</param>
        /// <param name="lineBreakTags">The element tag names that cause a new line to be inserted, defaults to "br" and "p" if none are specified, only applicable if <paramref name="multiline"/> is true.</param>
        /// <returns>The unescaped inner text of a node, optionally with new lines inserted for line-breaking elements.</returns>
        public static string GetInnerText(this HtmlNode node, bool multiline = false, string newLine = null, params string[] lineBreakTags)
        {
            if (multiline)
            {
                if (lineBreakTags == null || !lineBreakTags.Any())
                    lineBreakTags = _defaultLineBreakTags;
                var n = HtmlNode.CreateNode(node.OuterHtml);
                foreach (var lineBreakTag in lineBreakTags)
                    foreach (var lineBreakNode in n.SelectSafeNodes($"//{lineBreakTag.ToLowerInvariant()}").ToList())
                        lineBreakNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(_newLineToken + (lineBreakNode.InnerText ?? "") + _newLineToken), lineBreakNode);
                node = n;
            }
            var text = HtmlEntity.DeEntitize(node.InnerText).Replace("\r", "").Replace("\n", " ").Replace(_newLineToken, newLine ?? Environment.NewLine).TrimAndCollapse(multiline, newLine);
            return text;
        }
    }
}

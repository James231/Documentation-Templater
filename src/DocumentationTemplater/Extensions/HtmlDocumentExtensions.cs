// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace DocumentationTemplater.Extensions
{
    /// <summary>
    /// Extensions for <see cref="HtmlDocument"/> class.
    /// </summary>
    public static class HtmlDocumentExtensions
    {
        /// <summary>
        /// Returns a list of all nodes in the document of the given type.
        /// </summary>
        /// <param name="doc">The document to search through.</param>
        /// <param name="nodeType">The node type to search for.</param>
        /// <returns>Returns list of all nodes of given type.</returns>
        public static List<HtmlNode> GetAllNodesOfType(this HtmlDocument doc, string nodeType)
        {
            List<HtmlNode> nodes = doc.DocumentNode.Descendants(nodeType).ToList();
            return nodes;
        }

        /// <summary>
        /// Removes comments from HTML document.
        /// </summary>
        /// <param name="doc">HTML document to remove comments from.</param>
        public static void RemoveComments(this HtmlDocument doc)
        {
            var comments = doc.DocumentNode.Descendants()
                .OfType<HtmlCommentNode>()
                .Where(c => !c.Comment.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var comment in comments)
            {
                comment.Remove();
            }
        }
    }
}

// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using HtmlAgilityPack;

namespace DocumentationTemplater.Extensions
{
    public static class HtmlNodeExtensions
    {
        /// <summary>
        /// Returns the first child node of a given node with a given name.
        /// </summary>
        /// <param name="node">Node to search children of.</param>
        /// <param name="name">Name to search for.</param>
        /// <returns>First child node found with given name.</returns>
        public static HtmlNode FindFirstChild(this HtmlNode node, string name)
        {
            foreach (HtmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name.ToLower() == name.ToLower())
                {
                    return childNode;
                }
            }

            return null;
        }
    }
}

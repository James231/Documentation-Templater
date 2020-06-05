// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using WebMarkupMin.Core;

namespace DocumentationTemplater
{
    /// <summary>
    /// Utility Methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// HTML minification.
        /// </summary>
        /// <param name="html">Code to minify.</param>
        /// <returns>Minified html.</returns>
        public static string MinifyHtml(string html)
        {
            var settings = new HtmlMinificationSettings();
            var cssMinifier = new KristensenCssMinifier();
            var jsMinifier = new CrockfordJsMinifier();
            var htmlMinifier = new HtmlMinifier(settings, cssMinifier, jsMinifier);

            MarkupMinificationResult result = htmlMinifier.Minify(
                html,
                generateStatistics: false);
            if (result.Errors.Count == 0)
            {
                return result.MinifiedContent;
            }
            else
            {
                return html;
            }
        }

        /// <summary>
        /// Read full content of a file.
        /// </summary>
        /// <param name="filePath">Path of file to read.</param>
        /// <returns>Full content of file.</returns>
        public static string GetFullFileConent(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Removes all files and folders from a given directory.
        /// </summary>
        /// <param name="directory">Directory to remove files and folders from.</param>
        public static void ClearDirectory(string directory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                fileInfo.Delete();
            }

            foreach (DirectoryInfo subDirInfo in directoryInfo.GetDirectories())
            {
                subDirInfo.Delete(true);
            }
        }

        /// <summary>
        /// Returns a new list with same entries as given list.
        /// </summary>
        /// <param name="arr">List to clone.</param>
        /// <returns>Returns new list.</returns>
        public static IEnumerable<HtmlNode> CloneListNotValues(IEnumerable<HtmlNode> arr)
        {
            List<HtmlNode> newList = new List<HtmlNode>();
            foreach (HtmlNode node in arr)
            {
                newList.Add(node);
            }

            return newList;
        }
    }
}

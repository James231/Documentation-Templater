// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DocumentationTemplater.Extensions;
using DocumentationTemplater.Models;
using HtmlAgilityPack;
using Markdig;

namespace DocumentationTemplater
{
    /// <summary>
    /// Responsible for logic which only applies to pages.
    /// </summary>
    public static class PageManager
    {
        /// <summary>
        /// Loads file orders from files in directories.
        /// </summary>
        /// <param name="rootFolder">The root folder representing the Input folder.</param>
        public static void LoadOrders(SidenavFolder rootFolder)
        {
            string folderPath = rootFolder.InputFolderPath;
            string orderConfigPath = Path.Combine(folderPath, "order.config");
            if (File.Exists(orderConfigPath))
            {
                string configContents = Utils.GetFullFileConent(orderConfigPath);
                string[] lines = configContents.Split(Configuration.NewlineChars, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    int spacePos = line.IndexOf(' ');
                    if (spacePos != -1)
                    {
                        int order;
                        if (int.TryParse(line.Substring(0, spacePos), out order))
                        {
                            string fileName = line.Substring(spacePos + 1);
                            string inputFilePath = Path.Combine(rootFolder.InputFolderPath, fileName);
                            if (File.Exists(inputFilePath))
                            {
                                foreach (SidenavFile file in rootFolder.Files)
                                {
                                    if (file.InputFilePath == inputFilePath)
                                    {
                                        if (file.Order == 0)
                                        {
                                            file.Order = order;
                                        }
                                    }
                                }
                            }

                            if (Directory.Exists(inputFilePath))
                            {
                                foreach (SidenavFolder folder in rootFolder.Folders)
                                {
                                    if (folder.InputFolderPath == inputFilePath)
                                    {
                                        if (folder.Order == 0)
                                        {
                                            folder.Order = order;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (SidenavFolder folder in rootFolder.Folders)
            {
                LoadOrders(folder);
            }
        }

        /// <summary>
        /// Deserializes the Properties and html content which appear at the start of a file.
        /// </summary>
        /// <param name="file">The file to get the properties of.</param>
        public static void LoadProperties(SidenavFile file)
        {
            if (file.FileType == FileType.Markdown)
            {
                LoadMarkdownPageProperties(file);
            }
            else
            {
                LoadHtmlPageProperties(file);
            }
        }

        private static void LoadMarkdownPageProperties(SidenavFile file)
        {
            List<PProperty> properties = new List<PProperty>();
            string fullContent = Utils.GetFullFileConent(file.InputFilePath);

            string[] lines = fullContent.Split(Configuration.NewlineChars, StringSplitOptions.None);
            if (lines.Length == 0)
            {
                file.SetContent(properties, string.Empty);
                return;
            }

            int lineNum = 0;
            Regex regex = new Regex("Property:*,*");
            while (regex.IsMatch(lines[lineNum]) && lineNum < lines.Length)
            {
                string data = lines[lineNum].Substring(9);
                string[] datas = data.Split(',');
                string propertyName = datas[0].Trim();
                string propertyValue = datas[1].Trim();
                if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
                {
                    PProperty property = new PProperty(propertyName, propertyValue);
                    properties.Add(property);
                }

                lineNum++;
            }

            StringBuilder builder = new StringBuilder();
            bool first = true;
            for (int i = lineNum; i < lines.Length; i++)
            {
                if (!first)
                {
                    builder.Append(Configuration.DefaultNewlineChar);
                }

                builder.Append(lines[i]);
                first = false;
            }

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var markdown = Markdown.ToHtml(builder.ToString(), pipeline);

            file.SetContent(properties, markdown);
        }

        private static void LoadHtmlPageProperties(SidenavFile file)
        {
            List<PProperty> properties = new List<PProperty>();
            string fullContent = Utils.GetFullFileConent(file.InputFilePath);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(fullContent);
            doc.RemoveComments();

            HtmlNode[] propertyNodes = doc.DocumentNode.Descendants("PProperty").ToArray();
            foreach (HtmlNode node in propertyNodes)
            {
                string propertyName = HttpUtility.UrlEncode(node.GetAttributeValue("name", string.Empty));
                string propertyValue = node.GetAttributeValue("value", string.Empty);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    PProperty property = new PProperty(propertyName, propertyValue);
                    properties.Add(property);
                }

                node.Remove();
            }

            file.SetContent(properties, doc.DocumentNode.OuterHtml);
        }
    }
}

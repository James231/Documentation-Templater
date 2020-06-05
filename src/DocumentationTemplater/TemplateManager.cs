// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using DocumentationTemplater.Extensions;
using DocumentationTemplater.Models;
using HtmlAgilityPack;

namespace DocumentationTemplater
{
    public static class TemplateManager
    {
        /// <summary>
        /// Loads templates from the template folder.
        /// </summary>
        /// <param name="templateFolderPath">Folder containing all templates.</param>
        /// <returns>List of templates.</returns>
        public static List<Template> LoadTemplates(string templateFolderPath)
        {
            List<Template> templates = new List<Template>();
            DirectoryInfo templateDirectory = new DirectoryInfo(templateFolderPath);
            foreach (FileInfo fileInfo in templateDirectory.GetFiles())
            {
                string fileName = fileInfo.FullName;
                if (fileName.Length < 5)
                {
                    continue;
                }

                if (fileName.Substring(fileName.Length - 5) == ".html")
                {
                    string templateContent = Utils.GetFullFileConent(fileName);
                    bool acceptsContent = templateContent.Contains("@ChildContent;");

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(templateContent);
                    doc.RemoveComments();
                    string templateFileNameWithoutExtension = fileInfo.Name.Substring(0, fileInfo.Name.Length - Path.GetExtension(fileInfo.Name).Length);
                    string templateTagName = HttpUtility.UrlEncode(templateFileNameWithoutExtension);
                    List<TemplateProperty> properties = new List<TemplateProperty>();
                    HtmlNode[] nameNodes = doc.DocumentNode.Descendants("TName").ToArray();
                    if (nameNodes.Length > 0)
                    {
                        templateTagName = HttpUtility.UrlEncode(nameNodes[0].GetAttributeValue("name", templateTagName));
                    }

                    foreach (HtmlNode node in nameNodes)
                    {
                        node.Remove();
                    }

                    HtmlNode[] propertyNodes = doc.DocumentNode.Descendants("TProperty").ToArray();
                    foreach (HtmlNode node in propertyNodes)
                    {
                        string propertyName = HttpUtility.UrlEncode(node.GetAttributeValue("name", string.Empty));
                        string propertyDefault = HttpUtility.UrlEncode(node.GetAttributeValue("default", null));
                        if (!string.IsNullOrEmpty(propertyName))
                        {
                            TemplateProperty property = new TemplateProperty(propertyName, propertyDefault);
                            properties.Add(property);
                        }

                        node.Remove();
                    }

                    Template template = new Template(templateTagName, fileName, doc.DocumentNode.OuterHtml, acceptsContent);
                    template.Properties = properties;
                    templates.Add(template);
                }
            }

            return templates;
        }

        /// <summary>
        /// Injects template source code into html template elements.
        /// </summary>
        /// <param name="htmlContent">String to inject templates code into.</param>
        /// <param name="templates">List of templates.</param>
        /// <returns>Returns html with all template element code added.</returns>
        public static string InjectTemplates(string htmlContent, List<Template> templates)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            bool injectedTemplate = true;

            while (injectedTemplate == true)
            {
                injectedTemplate = false;
                foreach (Template template in templates)
                {
                    if (template.ElementName == "Page")
                    {
                        continue;
                    }

                    List<HtmlNode> templateNodes = doc.GetAllNodesOfType(template.ElementName);
                    foreach (HtmlNode templateNode in templateNodes)
                    {
                        string templateCode = template.TemplateContent;
                        if (template.AcceptsContent)
                        {
                            StringBuilder childContent = new StringBuilder();
                            IEnumerable<HtmlNode> childContentNodes = templateNode.ChildNodes;
                            foreach (HtmlNode node in childContentNodes)
                            {
                                childContent.Append(node.OuterHtml);
                            }

                            templateCode = templateCode.Replace("@ChildContent;", childContent.ToString());
                        }

                        // inject template properties
                        foreach (TemplateProperty templateProperty in template.Properties)
                        {
                            string value = templateNode.GetAttributeValue(templateProperty.Name, null);
                            if (value != null)
                            {
                                templateCode = templateCode.Replace($"@Template.{templateProperty.Name};", value);
                            }

                            if (value == null && templateProperty.DefaultValue != null)
                            {
                                templateCode = templateCode.Replace($"@Template.{templateProperty.Name};", templateProperty.DefaultValue);
                            }
                        }

                        HtmlDocument tempDoc = new HtmlDocument();
                        tempDoc.LoadHtml(templateCode);
                        foreach (HtmlNode childNode in tempDoc.DocumentNode.ChildNodes)
                        {
                            templateNode.ParentNode.InsertBefore(childNode, templateNode);
                        }

                        injectedTemplate = true;
                        templateNode.ParentNode.RemoveChild(templateNode);
                    }
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Returns the template with the given element name. Null if doesn't exist.
        /// </summary>
        /// <param name="elementName">The template name to look for.</param>
        /// <param name="templates">The list of templates to go through.</param>
        /// <returns>Returns template with given element name, or null.</returns>
        public static Template GetTemplateByElementName(string elementName, List<Template> templates)
        {
            return templates.FirstOrDefault(a => a.ElementName == elementName);
        }

        /// <summary>
        /// Checks the template directory has required template files.
        /// </summary>
        /// <param name="templateDirectory">Root directory containing all template files.</param>
        /// <returns>True if all required template files are present.</returns>
        public static bool CheckRequiredTemplatesExist(string templateDirectory)
        {
            foreach (string fileName in Configuration.RequiredTemplateFiles)
            {
                string filePath = Path.Combine(templateDirectory, fileName);
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Missing required template file {fileName}.");
                    return false;
                }
            }

            return true;
        }
    }
}

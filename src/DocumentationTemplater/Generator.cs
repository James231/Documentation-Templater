// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using DocumentationTemplater.Extensions;
using DocumentationTemplater.Models;

namespace DocumentationTemplater
{
    /// <summary>
    /// Responsible for creating documentation.
    /// </summary>
    public class Generator
    {
        private readonly string inputFolderPath;
        private readonly string outputFolderPath;
        private readonly string templateFolderPath;
        private List<Template> templates;
        private SidenavFolder rootFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="inputPath">Directory containing input files.</param>
        /// <param name="outputPath">Directory containing output files.</param>
        /// <param name="templatePath">Directory containing template files.</param>
        public Generator(string inputPath, string outputPath, string templatePath)
        {
            inputFolderPath = inputPath;
            outputFolderPath = outputPath;
            templateFolderPath = templatePath;
        }

        /// <summary>
        /// Generates documentation.
        /// </summary>
        public void Generate()
        {
            if (!TemplateManager.CheckRequiredTemplatesExist(templateFolderPath))
            {
                return;
            }

            Utils.ClearDirectory(outputFolderPath);
            rootFolder = TreeParser.LoadRootInputFolder(inputFolderPath);
            PageManager.LoadOrders(rootFolder);
            templates = TemplateManager.LoadTemplates(templateFolderPath);
            BuildFolder(rootFolder);
        }

        /// <summary>
        /// Builds and writes html pages for entire folder (including files and subdirectories).
        /// </summary>
        /// <param name="folder">Folder to output the content of.</param>
        private void BuildFolder(SidenavFolder folder)
        {
            string thisOutputFolderPath = Path.Combine(outputFolderPath, folder.GetAbsoluteFolderPath());
            Directory.CreateDirectory(thisOutputFolderPath);

            foreach (SidenavFolder subFolder in folder.Folders)
            {
                BuildFolder(subFolder);
            }

            foreach (SidenavFile file in folder.Files)
            {
                BuildFile(file);
            }
        }

        /// <summary>
        /// Builds and writes a single documentation page file.
        /// </summary>
        /// <param name="file">File to build.</param>
        private void BuildFile(SidenavFile file)
        {
            string outputFilePath = Path.Combine(outputFolderPath, file.GetAbsoluteFilePath());
            string innerfileContent = file.StringContent;
            Template pageTemplate = TemplateManager.GetTemplateByElementName("Page", templates);
            string pagefileContent = pageTemplate.TemplateContent;
            pagefileContent = pagefileContent.Replace("@Page.Id;", file.Id);
            foreach (PProperty property in file.Properties)
            {
                pagefileContent = pagefileContent.Replace($"@Page.{property.Name};", property.Value);
            }

            if (pagefileContent == null)
            {
                throw new MissingFieldException("Could not find Page template");
            }

            string combinedFileContent = pagefileContent.Replace("@ChildContent;", innerfileContent);
            combinedFileContent = TemplateManager.InjectTemplates(combinedFileContent, templates);
            combinedFileContent = SpecialTagManager.InjectSpecialTags(combinedFileContent, rootFolder, file);

            using (StreamWriter writer = new StreamWriter(outputFilePath, false))
            {
                writer.Write(Utils.MinifyHtml(combinedFileContent));
            }
        }
    }
}

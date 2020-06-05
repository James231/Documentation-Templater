// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DocumentationTemplater.Models
{
    /// <summary>
    /// Model for Template files.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="filePath">File path of the template.</param>
        /// <param name="content">String content of template file.</param>
        /// <param name="acceptsContent">Should the element accept content.</param>
        public Template(string elementName, string filePath, string content, bool acceptsContent)
        {
            ElementName = elementName;
            FilePath = filePath;
            TemplateContent = content;
            AcceptsContent = acceptsContent;
            Properties = new List<TemplateProperty>();
        }

        /// <summary>
        /// Gets or sets the name of the template element.
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// Gets or sets the file path of the template element.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the template accepts child content.
        /// </summary>
        public bool AcceptsContent { get; set; }

        /// <summary>
        /// Gets or sets the string content of the template.
        /// </summary>
        public string TemplateContent { get; set; }

        /// <summary>
        /// Gets or sets properties of the template.
        /// </summary>
        public List<TemplateProperty> Properties { get; set; }
    }
}

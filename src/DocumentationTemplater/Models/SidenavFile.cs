// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DocumentationTemplater.Models
{
    /// <summary>
    /// Model for file.
    /// </summary>
    public class SidenavFile : SidenavElement
    {
        private List<PProperty> properties;
        private string stringContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SidenavFile"/> class.
        /// </summary>
        /// <param name="name">Name for file to show in sidenav.</param>
        /// <param name="inputFilePath">Full path to file in input directory.</param>
        /// <param name="fileType">Type of file.</param>
        /// <param name="link">Relative html link to file (on base domain).</param>
        /// <param name="parent">Sidenav parent folder.</param>
        public SidenavFile(string name, FileType fileType, string inputFilePath, string link, SidenavFolder parent)
        {
            FileName = name;
            FileType = fileType;
            InputFilePath = inputFilePath;
            RelativeHtmlLink = link;
            Parent = parent;
            Id = Guid.NewGuid().ToString().Substring(10);
        }

        /// <summary>
        /// Gets or sets name for this file in the sidenav.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file type.
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Gets or sets the file path for the file in the input directory.
        /// </summary>
        public string InputFilePath { get; set; }

        /// <summary>
        /// Gets or sets relative HTML link for this file in the sidenav.
        /// </summary>
        public string RelativeHtmlLink { get; set; }

        /// <summary>
        /// Gets or sets the parent folder for this file in the sidenav.
        /// </summary>
        public SidenavFolder Parent { get; set; }

        /// <summary>
        /// Gets or sets a unique random id for the file.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the properties with initialized values in the file.
        /// </summary>
        public List<PProperty> Properties
        {
            get
            {
                if (properties == null)
                {
                    PageManager.LoadProperties(this);
                }

                return properties;
            }

            set
            {
                properties = value;
            }
        }

        /// <summary>
        /// Gets or sets the string content for the file (excluding properties).
        /// </summary>
        public string StringContent
        {
            get
            {
                if (stringContent == null)
                {
                    PageManager.LoadProperties(this);
                }

                return stringContent;
            }

            set
            {
                stringContent = value;
            }
        }

        /// <summary>
        /// Sets the content of the file model.
        /// </summary>
        /// <param name="props">Properties from file.</param>
        /// <param name="content">Remaining html content from file.</param>
        public void SetContent(List<PProperty> props, string content)
        {
            stringContent = content;
            properties = props;

            foreach (PProperty property in props)
            {
                if (property.Name.ToLower() == "order")
                {
                    int orderNum;
                    if (int.TryParse(property.Value, out orderNum))
                    {
                        Order = orderNum;
                    }
                }
            }
        }
    }
}

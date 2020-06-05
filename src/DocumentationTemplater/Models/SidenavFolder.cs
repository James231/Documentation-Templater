// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DocumentationTemplater.Models
{
    /// <summary>
    /// Model for folder.
    /// </summary>
    public class SidenavFolder : SidenavElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SidenavFolder"/> class.
        /// </summary>
        /// <param name="name">Name of Sidenav folder.</param>
        /// <param name="inputFolderPath">Path to folder in the input directory.</param>
        /// <param name="htmlLink">Relative html link to file (on base domain).</param>
        /// <param name="parent">Sidenav parent folder.</param>
        public SidenavFolder(string name, string inputFolderPath, string htmlLink, SidenavFolder parent)
        {
            FolderName = name;
            InputFolderPath = inputFolderPath;
            RelativeHtmlLink = htmlLink;
            Folders = new List<SidenavFolder>();
            Files = new List<SidenavFile>();
            Parent = parent;
            Id = Guid.NewGuid().ToString().Substring(10);
        }

        /// <summary>
        /// Gets or sets name for this folder in the sidenav.
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or sets the path to the folder in the input directory.
        /// </summary>
        public string InputFolderPath { get; set; }

        /// <summary>
        /// Gets or sets relative HTML link for this file in the sidenav.
        /// </summary>
        public string RelativeHtmlLink { get; set; }

        /// <summary>
        /// Gets or sets subfolders to be listed under this folder in the sidenav.
        /// </summary>
        public List<SidenavFolder> Folders { get; set; }

        /// <summary>
        /// Gets or sets files to be listed under this folder in the sidenav.
        /// </summary>
        public List<SidenavFile> Files { get; set; }

        /// <summary>
        /// Gets or sets the parent folder for this folder in the sidenav.
        /// </summary>
        public SidenavFolder Parent { get; set; }

        /// <summary>
        /// Gets or sets a unique id for this folder.
        /// </summary>
        public string Id { get; set; }
    }
}

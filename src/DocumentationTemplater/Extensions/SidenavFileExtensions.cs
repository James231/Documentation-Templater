// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.IO;
using System.Text;
using DocumentationTemplater.Models;

namespace DocumentationTemplater.Extensions
{
    public static class SidenavFileExtensions
    {
        /// <summary>
        /// Gets the absolute link to a page in sidenav.
        /// </summary>
        /// <param name="file">File to get link from.</param>
        /// <returns>Absolute link starting with '/'.</returns>
        public static string GetAbsoluteLink(this SidenavFile file)
        {
            StringBuilder builder = new StringBuilder(file.RelativeHtmlLink);
            SidenavFolder parentFolder = file.Parent;
            builder.Insert(0, $"{parentFolder.RelativeHtmlLink}/");
            while (parentFolder.Parent != null)
            {
                parentFolder = parentFolder.Parent;
                builder.Insert(0, $"{parentFolder.RelativeHtmlLink}/");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns absolute file path of the file inside Input directory.
        /// </summary>
        /// <param name="file">File to get the path of.</param>
        /// <returns>Absolute file path.</returns>
        public static string GetAbsoluteFilePath(this SidenavFile file)
        {
            if (file.Parent.FolderName == "root")
            {
                return file.RelativeHtmlLink;
            }
            else
            {
                return Path.Combine(file.Parent.GetAbsoluteFolderPath(), file.RelativeHtmlLink);
            }
        }
    }
}

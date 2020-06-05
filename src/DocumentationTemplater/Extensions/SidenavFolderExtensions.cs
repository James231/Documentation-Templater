// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.IO;
using DocumentationTemplater.Models;

namespace DocumentationTemplater.Extensions
{
    public static class SidenavFolderExtensions
    {
        /// <summary>
        /// Returns absolute folder path of the folder inside Input directory.
        /// </summary>
        /// <param name="folder">Folder to get the path of.</param>
        /// <returns>Absolute folder path.</returns>
        public static string GetAbsoluteFolderPath(this SidenavFolder folder)
        {
            if (folder.FolderName == "root")
            {
                return string.Empty;
            }
            else
            {
                return Path.Combine(folder.Parent.GetAbsoluteFolderPath(), folder.RelativeHtmlLink);
            }
        }
    }
}

// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.IO;
using System.Web;
using DocumentationTemplater.Models;

namespace DocumentationTemplater
{
    /// <summary>
    /// Scans input directory to load <see cref="SidenavFolder"/> and <see cref="SidenavFile"/> instances.
    /// </summary>
    public static class TreeParser
    {
        /// <summary>
        /// Returns sidenav content for root input folder.
        /// </summary>
        /// <param name="inputDirectory">Root directory containing all input files.</param>
        /// <returns>Root <see cref="SidenavFolder"/> instance.</returns>
        public static SidenavFolder LoadRootInputFolder(string inputDirectory)
        {
            return LoadSidenavFolder("root", inputDirectory, null);
        }

        private static SidenavFolder LoadSidenavFolder(string sidenavFolderName, string directory, SidenavFolder parent)
        {
            string actualFolderName = Path.GetFileName(directory);
            if (sidenavFolderName == "root")
            {
                actualFolderName = string.Empty;
            }

            SidenavFolder parentFolder = new SidenavFolder(sidenavFolderName, directory, HttpUtility.UrlEncode(actualFolderName), parent);
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                if (fileInfo.Extension == ".html" || fileInfo.Extension == ".md")
                {
                    string fileLink = HttpUtility.UrlEncode(fileInfo.Name);
                    FileType fileType = FileType.Html;
                    if (fileInfo.Extension == ".md")
                    {
                        fileLink = fileLink.Substring(0, fileLink.Length - 2) + "html";
                        fileType = FileType.Markdown;
                    }

                    SidenavFile file = new SidenavFile(fileInfo.Name, fileType, fileInfo.FullName, fileLink, parentFolder);
                    parentFolder.Files.Add(file);
                }
            }

            foreach (DirectoryInfo subDirInfo in directoryInfo.GetDirectories())
            {
                SidenavFolder folder = LoadSidenavFolder(subDirInfo.Name, subDirInfo.FullName, parentFolder);
                if (folder.Files.Count != 0 || folder.Folders.Count != 0)
                {
                    parentFolder.Folders.Add(folder);
                }
            }

            return parentFolder;
        }
    }
}

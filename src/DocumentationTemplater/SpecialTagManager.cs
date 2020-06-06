// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentationTemplater.Extensions;
using DocumentationTemplater.Models;
using HtmlAgilityPack;

namespace DocumentationTemplater
{
    /// <summary>
    /// Handles methods relating to special tags in page template. i.e. <ForAllFolders> and important subtags
    /// </summary>
    public static class SpecialTagManager
    {
        /// <summary>
        /// Replaces special tags in html.
        /// </summary>
        /// <param name="htmlContent">Html to replace tags in.</param>
        /// <param name="rootFolder">Root SidenavFolder.</param>
        /// <param name="currentFile">The <see cref="SidenavFile"/> whose page is currently being built.</param>
        /// <returns>Html with special tags corrected.</returns>
        public static string InjectSpecialTags(string htmlContent, SidenavFolder rootFolder, SidenavFile currentFile)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            List<HtmlNode> allFoldersNodes = doc.GetAllNodesOfType("ForAllFolders");
            foreach (HtmlNode node in allFoldersNodes)
            {
                HtmlNode allSubFoldersNode = node.FindFirstChild("ForeachSubFolder");
                HtmlNode allPagesNode = node.FindFirstChild("ForeachPage");

                StringBuilder allSubFolderContent = new StringBuilder();
                if (allSubFoldersNode != null)
                {
                    foreach (HtmlNode subNode in allSubFoldersNode.ChildNodes)
                    {
                        allSubFolderContent.Append(subNode.OuterHtml);
                    }
                }

                StringBuilder allPagesContent = new StringBuilder();
                if (allPagesNode != null)
                {
                    foreach (HtmlNode pagesNode in allPagesNode.ChildNodes)
                    {
                        allPagesContent.Append(pagesNode.OuterHtml);
                    }
                }

                string rootAllFoldersContent = GetHtmlToReplaceForAllFoldersTag(rootFolder, currentFile, allSubFolderContent.ToString(), allPagesContent.ToString(), 1);
                HtmlDocument newDoc = new HtmlDocument();
                newDoc.LoadHtml(rootAllFoldersContent);
                foreach (HtmlNode allFoldersNode in newDoc.DocumentNode.ChildNodes)
                {
                    node.ParentNode.InsertBefore(allFoldersNode, node);
                }

                node.ParentNode.RemoveChild(node);
            }

            doc.RemoveComments();
            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Generates html content which should replace the <ForAllFolders> tag.
        /// </summary>
        /// <param name="rootFolder">Sidenav folder which represents as the Input folder.</param>
        /// <param name="currentFile">The file currently being built for.</param>
        /// <param name="allSubFolderContent">Html content of <ForAllSubFolders> tag to be repeated.</param>
        /// <param name="allPagesContent">Html content of <ForAllSubPages> tag to be repeated.</param>
        /// <param name="layer">Integer representing the depth of the current folder from the Input folder.</param>
        /// <returns>Returns html content to replace the <ForAllFolders> tag.</returns>
        private static string GetHtmlToReplaceForAllFoldersTag(SidenavFolder rootFolder, SidenavFile currentFile, string allSubFolderContent, string allPagesContent, int layer)
        {
            StringBuilder forAllContent = new StringBuilder();

            List<SidenavElement> elmts = new List<SidenavElement>();
            foreach (SidenavFolder folder in rootFolder.Folders)
            {
                elmts.Add(folder);
            }

            foreach (SidenavFile file in rootFolder.Files)
            {
                elmts.Add(file);
            }

            IOrderedEnumerable<SidenavElement> orderedElmts = elmts.OrderBy(o => o.Order);

            foreach (SidenavElement elmt in orderedElmts)
            {
                if (elmt is SidenavFolder)
                {
                    SidenavFolder folder = (SidenavFolder)elmt;
                    string folderContent = GetHtmlToReplaceForAllFoldersTag(folder, currentFile, allSubFolderContent, allPagesContent, layer + 1);
                    string allSubFolderFilledContent = allSubFolderContent.Replace("@SubFolder.Name;", folder.FolderName);
                    allSubFolderFilledContent = allSubFolderFilledContent.Replace("@SubFolder.Layer;", layer.ToString());
                    allSubFolderFilledContent = HandleThisFolderSpecialTags(allSubFolderFilledContent, folder, currentFile);
                    allSubFolderFilledContent = allSubFolderFilledContent.Replace("@SubFolder.Recursive;", folderContent);
                    forAllContent.Append(allSubFolderFilledContent);
                }
                else
                {
                    SidenavFile file = (SidenavFile)elmt;
                    string pageContent = allPagesContent;
                    pageContent = HandleThisPageSpecialTags(pageContent, file, currentFile);
                    foreach (PProperty property in file.Properties)
                    {
                        pageContent = pageContent.Replace($"@SubPage.{property.Name};", property.Value);
                    }

                    pageContent = pageContent.Replace("@SubPage.Layer;", layer.ToString());
                    pageContent = pageContent.Replace("@SubPage.Link;", file.GetAbsoluteLink());
                    pageContent = pageContent.Replace("@SubPage.Id;", file.Id);
                    forAllContent.Append(pageContent);
                }
            }

            return forAllContent.ToString();
        }

        /// <summary>
        /// Replaces html <IsThisPage> and <IsNotThisPage> with html content (within <ForAllPages> tag).
        /// </summary>
        /// <param name="forAllPagesContent">Html content to replace <ForAllPages> tag.</param>
        /// <param name="currentFileInLoop">Current file we are copying an instance of <ForAllPages> for.</param>
        /// <param name="currentPageFile">Current file being built.</param>
        /// <returns>Returns Html content with <IsThisPage> and <IsNotThisPage> converted into pure Html.</returns>
        private static string HandleThisPageSpecialTags(string forAllPagesContent, SidenavFile currentFileInLoop, SidenavFile currentPageFile)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(forAllPagesContent);
            IEnumerable<HtmlNode> show, hide;
            if (currentFileInLoop == currentPageFile)
            {
                show = Utils.CloneListNotValues(doc.DocumentNode.Descendants("IsThisPage".ToLower(Configuration.Culture)));
                hide = Utils.CloneListNotValues(doc.DocumentNode.Descendants("IsNotThisPage".ToLower(Configuration.Culture)));
            }
            else
            {
                hide = Utils.CloneListNotValues(doc.DocumentNode.Descendants("IsThisPage".ToLower(Configuration.Culture)));
                show = Utils.CloneListNotValues(doc.DocumentNode.Descendants("IsNotThisPage".ToLower(Configuration.Culture)));
            }

            foreach (HtmlNode showNode in show)
            {
                foreach (HtmlNode childNode in showNode.ChildNodes)
                {
                    showNode.ParentNode.InsertBefore(childNode, showNode);
                }

                showNode.ParentNode.RemoveChild(showNode);
            }

            foreach (HtmlNode hideNode in hide)
            {
                hideNode.ParentNode.RemoveChild(hideNode);
            }

            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Replaces html <IsThisFolder> and <IsNotThisFolder> with html content (within <ForAllSubFolders> tag).
        /// </summary>
        /// <param name="forAllSubFoldersContent">Html content to replace <ForAllSubFolders> tag.</param>
        /// <param name="currentFolderInLoop">Current folder we are copying an instance of <ForAllPages> for.</param>
        /// <param name="currentPageFile">Current file being built.</param>
        /// <returns>Returns Html content with <IsThisFolder> and <IsNotThisFolder> converted into pure Html.</returns>
        private static string HandleThisFolderSpecialTags(string forAllSubFoldersContent, SidenavFolder currentFolderInLoop, SidenavFile currentPageFile)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(forAllSubFoldersContent);
            IEnumerable<HtmlNode> show, hide;
            bool isThisFolder = false;
            SidenavFolder loopFolder = currentPageFile.Parent;
            if (loopFolder == currentFolderInLoop)
            {
                isThisFolder = true;
            }

            while (loopFolder.Parent != null && !isThisFolder)
            {
                if (loopFolder.Parent == currentFolderInLoop)
                {
                    isThisFolder = true;
                }

                loopFolder = loopFolder.Parent;
            }

            if (isThisFolder)
            {
                show = Utils.CloneListNotValues(doc.DocumentNode.Descendants("IsThisFolder".ToLower(Configuration.Culture)));
                hide = Utils.CloneListNotValues(doc.DocumentNode.Descendants("IsNotThisFolder".ToLower(Configuration.Culture)));
            }
            else
            {
                hide = Utils.CloneListNotValues(doc.DocumentNode.Descendants("IsThisFolder".ToLower(Configuration.Culture)));
                show = Utils.CloneListNotValues(doc.DocumentNode.Descendants("IsNotThisFolder".ToLower(Configuration.Culture)));
            }

            foreach (HtmlNode showNode in show)
            {
                foreach (HtmlNode childNode in showNode.ChildNodes)
                {
                    showNode.ParentNode.InsertBefore(childNode, showNode);
                }

                showNode.ParentNode.RemoveChild(showNode);
            }

            foreach (HtmlNode hideNode in hide)
            {
                hideNode.ParentNode.RemoveChild(hideNode);
            }

            return doc.DocumentNode.OuterHtml;
        }
    }
}

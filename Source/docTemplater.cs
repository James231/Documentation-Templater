using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;



/*

Debugging can be done using the VS command prompt. add -debug flag when compiling



finish the "Needs finishing"s with more content (tabs, tables, etc)

Nested tabs not working.
Problem is occuring as when the code sees ### Begin Tabs it looks for the next ### End Tabs
   but with nested tabs there will be another ### Begin Tabs and ### End Tabs first which it should ignore


checkout function "CreateFileMenuContent" and handle layers

Should be able to identify a certain tab as active
    separate templates for active tab?


Sidenav:
Check subsubdirectories are working
Have an active tag thingy on the active menu item (different files for active and not active)
the active menu item should have directory menus expanded
checkout function "CreateFileMenuContent" and handle layers

*/




namespace apitosandboxtool
{
	class MainClass
	{
		public static string[] templateFileNames = new string[12] {"page_template.html", "section_content.html", "section_header.html", "sidenav_directory.html", "sidenav_page.html", "table_cell.html", "table_heading.html", "table_parent.html", "table_row.html", "tabs_content.html", "tabs_parent.html", "tabs_tab.html"};

		// directorys do not end in a slash !!!
		public static string dirPath;
		public static string outputPath;
		public static string templatePath;

		// class representing the root directory. Directories will only contain the txt/html files we are dealing with
		public static MyDirectory root;

		// Counts the number of sections/tabs/tables/HTMLs we are using
		public static int baseItemNumber = 1;


#region Start Methods
		public static void Main (string[] args)
		{
			Generate();
		}

		public static void Generate () {
			GetDirs();
			ClearDirectory(outputPath);
			GenerateOutputDirectorys();
			GenerateHTMLFiles (dirPath, outputPath, root);
			CreateFilesInDirectory(root);
			CopyTemplateFilesToOutput();
		}

		public static void CreateFilesInDirectory (MyDirectory dir) {
			foreach(MyFile file in dir.files) {
				CreateFile(file);
			}
			foreach (MyDirectory d in dir.subdirs) {
				CreateFilesInDirectory(d);
			}
		}

		public static void CreateFile (MyFile file) {

			StreamReader reader = new StreamReader(file.txtPath);
			string contents = reader.ReadToEnd();
			reader.Close();
			List<BaseContent> content = ParseStringToContent(contents);
			//DebugTree(content);

			string projectName = CheckForContent(content, 1, "Project Name", "Project Name", file.htmlPath);
			string pageTitle = CheckForContent(content, 1, "Page Title", "Page Title", file.htmlPath);
			string version = CheckForContent(content, 1, "Version", "1.0.0", file.htmlPath);
			string mainContent = GetMainContent(content, file.htmlPath);


			string sideMenuItemsString = CreateSideMenuItems(root, "", 0, 1, file);

			string fileString = GetTemplateString("page_template", new string[5] {"PROJECT_NAME", "PAGE_TITLE", "SIDENAV_CONTENT", "MAIN_CONTENT", "VERSION"}, new string[5] {projectName, pageTitle, sideMenuItemsString, mainContent, version}, file.htmlPath);

			StreamWriter writer = new StreamWriter(file.htmlPath, false);
			writer.Write(fileString);
			writer.Close();
		}

		public static List<BaseContent> ParseStringToContent (string text) {
			string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

			// In this list we'll have all consecutive content lines joined together
			// so the strings should alternate between identifiers and content
			List<string> parts = new List<string>();

			string curPart = "";
			for (int i = 0; i < lines.Length; i++) {
				if (checkPrefix(lines[i].Trim(), "#")) {
					if (parts.Count != 0) {
						parts.Add(curPart);
					}
					parts.Add(lines[i]);
					curPart = "";
				} else {
					curPart = (curPart + "\n" + lines[i]).Trim();
				}
			}
			if (!string.IsNullOrEmpty(curPart)) {
				parts.Add(curPart);
			}

			return ParseListToContent(parts);
		}

		public static List<BaseContent> ParseListToContent (List<string> list) {
			List<BaseContent> returnList = new List<BaseContent>();

			for (int i = 0; i < list.Count; i++) {
				// find and handle the first identifier in the list
				if (checkPrefix(list[i].Trim(), "#")) {

					// The indicies between which we are going to handle all the lines
					int startIndex = i;
					int endIndex = i;

					// The line including hashes to handle
					string lineToHandle = list[i].Trim();

					// count number of hashes
					int numberOfHashes = 0;
					for (int j = 0; j < lineToHandle.Length; j++) {
						if (lineToHandle[j] != '#') {
							break;
						} else {
							numberOfHashes++;
						}
					}

					// Remove hashes
					lineToHandle = lineToHandle.Substring(numberOfHashes).Trim();

					// If it is not a begin statement then we just handle this line (and maybe the next one with the content)
					// If it is a begin statement we need to find where the statement ends and handle all lines in between
					if (!checkPrefix(lineToHandle,"Begin ")) {
						bool handleNextLine = false;
						if (list.Count > i+1) {
							if (!checkPrefix(list[i+1].Trim(), "#")) {
								handleNextLine = true;
							}
						}
						if (handleNextLine) {
							endIndex++;
							returnList.Add (new Content(numberOfHashes, lineToHandle, list[i+1].Trim()));
						} else {
							returnList.Add (new Content(numberOfHashes, lineToHandle, ""));
						}
					} else {
						string beginString = new String('#', numberOfHashes) + " Begin " + lineToHandle.Substring(6);
						string endString = new String('#', numberOfHashes) + " End " + lineToHandle.Substring(6);
						int numToIgnore = 0;
						if (list.Count > i+1) {
							for (int j = i+1; j < list.Count; j++) {
								if (list[j].Trim() == beginString) {
									numToIgnore++;
								} else {
									if (list[j].Trim() == endString) {
										if (numToIgnore > 0) {
											numToIgnore--;
										} else {
											endIndex = j;
											break;
										}
									}
								}
							}
						}
						if (endIndex == i) {
							endIndex = list.Count;
						}

						returnList.Add(new ContentParent(numberOfHashes, lineToHandle, ParseListToContent(list.GetRange(startIndex + 1, endIndex - startIndex - 1))));
					}

					List<string> continueList = list;
					for (int k = 0; k < endIndex - startIndex + 1; k++) {
						if (startIndex < continueList.Count) {
							continueList.RemoveAt(startIndex);
						}
					}

					returnList.AddRange(ParseListToContent(continueList));
					break;
				}
			}

			return returnList;
		}

		public static bool checkPrefix (string text, string prefix) {
			if (text.Length < prefix.Length) {
				return false;
			} else {
				if (text.Substring(0, prefix.Length) == prefix) {
					return true;
				}
			}
			return false;
		}

		public static string GetMainContent (List<BaseContent> contents, string filePath) {
			string retString = "";
			foreach (BaseContent cont in contents) {
				if ((cont is ContentParent)||((cont is Content) && (cont.name == "HTML") && (cont.numberOfHashes == 1))) {
					retString += cont.GetString(filePath);
					baseItemNumber++;
				}
			}
			return retString;
		}

		public static string CheckForContent (List<BaseContent> contents, int numberOfHashes, string name, string def, string filePath) {
			foreach (BaseContent content in contents) {
				if ((content.numberOfHashes == numberOfHashes) && (content.name == name)) {
					return content.GetString(filePath);
				}
			}
			return def;
		}
		
		public class BaseContent {
			public int numberOfHashes;
			public string name;

			public BaseContent (int num, string nam) {
				numberOfHashes = num;
				name = nam;
			}

			public virtual string GetString (string filePath) {
				return "";
			}
		}

		public class Content : BaseContent {
			public string content;

			public Content (int num, string nam, string c) : base(num, nam) {
				content = c;
			}

			public override string GetString (string filePath) {
				return content;
			}
		}

		public class ContentParent : BaseContent {
			public List<BaseContent> children;

			public ContentParent (int num, string nam, List<BaseContent> c) : base(num, nam) {
				children = c;
			}

			public override string GetString (string filePath) {
				if ((numberOfHashes == 2) && (name == "Begin Section")) {
					string sectionHeader = "";
					string sectionContent = "";
					foreach (BaseContent child in children) {
						if ((child.numberOfHashes == 1) && (child.name == "Heading")) {
							sectionHeader = child.GetString(filePath);
						}
						if ((child.numberOfHashes == 1) && (child.name == "Content")) {
							sectionContent = child.GetString(filePath);
						}
					}
					string returnString = "";
					if (!string.IsNullOrEmpty(sectionHeader)) {
						returnString += GetTemplateString("section_header", new string[1] {"HEADING"}, new string[1] {sectionHeader}, filePath);
					}
					if (!string.IsNullOrEmpty(sectionContent)) {
						returnString += GetTemplateString("section_content", new string[1] {"CONTENT"}, new string[1] {sectionContent}, filePath);
					}
					baseItemNumber++;
					return returnString;
				}
				if ((numberOfHashes == 3) && (name == "Begin Tabs")) {
					List<string> tabHeadings = new List<string>();
					List<string> tabContents = new List<string>();
					int itemNum = 1;
					foreach (BaseContent child in children) {
						if ((child is ContentParent) && (child.numberOfHashes == 2) && (child.name == "Begin Tab")) {
							ContentParent tabThings = (ContentParent)child;
							List<BaseContent> content = tabThings.children;
							string tabHead = "";
							string tabCont = "";
							foreach (BaseContent c in content) {
								if ((c.numberOfHashes == 1) && (c.name == "Tab Heading")) {
									tabHead = c.GetString(filePath);
								}
								if ((c is ContentParent)&&(c.numberOfHashes == 1)&&(c.name == "Begin Content")) {
									ContentParent p = (ContentParent)c;
									tabCont = GetMainContent(p.children, filePath);
								}
							}
							tabHeadings.Add(GetTemplateString("tabs_tab", new string[2] {"TAB NAME", "TAB NUMBER"}, new string[2] {tabHead, "" + itemNum}, filePath));
							tabContents.Add(GetTemplateString("tabs_content", new string[3] {"TAB NAME", "CONTENT", "TAB NUMBER"}, new string[3] {tabHead, tabCont, "" + itemNum}, filePath));
						}
						itemNum++;
					}
					string tabsString = string.Join("", tabHeadings.ToArray());
					string contentsString = string.Join("", tabContents.ToArray());
					string retString = GetTemplateString("tabs_parent", new string[2] {"TABS", "TAB CONTENT"}, new string[2] {tabsString, contentsString}, filePath);
					baseItemNumber++;
					return retString;
				}
				if ((numberOfHashes == 3) && (name == "Begin Table")) {
					List<string> headings = new List<string>();
					List<string> rowStrings = new List<string>();
					foreach (BaseContent child in children) {
						if ((child is Content) && (child.numberOfHashes == 1) && (child.name == "Heading")) {
							string headingName = child.GetString(filePath);
							headings.Add(GetTemplateString("table_heading", new string[1] {"HEADING"}, new string[1] {headingName}, filePath));
						}
						if ((child is ContentParent) && (child.numberOfHashes == 2) && (child.name == "Begin Row")) {
							ContentParent rowParent = (ContentParent)child;
							List<string> cells = new List<string>();
							foreach (BaseContent c in rowParent.children) {
								if ((c is Content) && (c.numberOfHashes == 1) && (c.name == "Cell")) {
									cells.Add(GetTemplateString("table_cell", new string[1] {"CONTENT"}, new string[1] {c.GetString(filePath)}, filePath));
								}
							}
							if (cells.Count > 0) {
								string cellsJoined = string.Join("", cells.ToArray());
								rowStrings.Add(GetTemplateString("table_row", new string[1] {"CELLS"}, new string[1] {cellsJoined}, filePath));
							}
						}
					}
					string headingString = string.Join("", headings.ToArray());
					string rowString = string.Join("", rowStrings.ToArray());
					string tableString = GetTemplateString("table_parent", new string[2] {"TABLE HEADINGS", "ROWS"}, new string[2] {headingString, rowString}, filePath);
					baseItemNumber++;
					return tableString;
				}


				///////////////////////////////////////////////////////////////////////////////
				// Needs finishing
				// If you need to get the string for general content (which may contain sections/tables/tabs etc)
				//      then use the GetMainContent function (passing in the children as the argument)
				//////////////////////////////////////////////////////////////////////////////


				return "";
			}
		}


		public static string CreateDirectoryMenuContent (MyDirectory dir, int itemNum, string dirChildContent, string filePath) {
			return GetTemplateString("sidenav_directory", new string[3] {"DIRECTORY_NAME", "ITEM_NUMBER", "SUBFILE_CONTENT"}, new string[3] {dir.dirName, "" + itemNum, dirChildContent}, filePath);
		}

		public static string CreateFileMenuContent (MyFile file, int layer, int itemNum, MyFile displayFile) {
			// Create the single menu item for the file
			// layer = 0 means the file is in the root directory
			// layer = 1 means its in a subdirectory
			// layer = 2 means its in a subsubdirectory
			// ... etc    You can use these layers for different nested colours
			string relativePath = GetRelativeURIPath(displayFile, file).Replace("\\", "/");
			return GetTemplateString("sidenav_page", new string[4] {"PAGE_NAME", "LAYER_NUMBER", "ITEM_NUMBER", "RELATIVE_FILE_PATH"}, new string[4] {file.fileName, "" + layer, "" + itemNum, relativePath}, displayFile.htmlPath);
		}

		public static string GetRelativeURIPath (MyFile from, MyFile to) {
			string fromPath = from.htmlPath;
			string toPath = to.htmlPath;
			return GetRelativeURIPath(fromPath, toPath);
    	}

    	public static string GetRelativeURIPath (string fromPath, string toPath) {
			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);
    		if (fromUri.Scheme != toUri.Scheme) { return "#"; } // path can't be made relative.

    		Uri relativeUri = fromUri.MakeRelativeUri(toUri);
    		String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

    		if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
    		{
    			relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    		}

    		return relativePath;
    	}

    	public static string CreateSideMenuItems (MyDirectory dir, string curString, int layer, int itemNum, MyFile displayfile) {
    		if (dir.dirName == "root") {
    			string dirChildContent = "";
	    		foreach (MyDirectory d in dir.subdirs) {
	    			itemNum++;
	    			layer++;
	    			dirChildContent = CreateSideMenuItems(d, dirChildContent, layer, itemNum, displayfile);
	    		}
	    		foreach (MyFile file in dir.files) {
	    			itemNum++;
	    			dirChildContent += CreateFileMenuContent(file, layer, itemNum, displayfile);
	    		}
	    		return dirChildContent;
    		} else {
	    		int itemNumToUse = itemNum;
	    		string dirChildContent = "";
	    		foreach (MyDirectory d in dir.subdirs) {
	    			itemNum++;
	    			layer++;
	    			dirChildContent = CreateSideMenuItems(d, dirChildContent, layer, itemNum, displayfile);
	    		}
	    		foreach (MyFile file in dir.files) {
	    			itemNum++;
	    			dirChildContent += CreateFileMenuContent(file, layer, itemNum, displayfile);
	    		}
	    		string newString = curString + CreateDirectoryMenuContent(dir, itemNumToUse, dirChildContent, displayfile.htmlPath);
	    		return newString;
	    	}
    	}

    	private static void GetDirs () {
    		string mainPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    		dirPath = mainPath + "\\Input";
    		outputPath = mainPath + "\\Output";
    		templatePath = mainPath + "\\Template";


    		root = new MyDirectory("root", dirPath, outputPath, new List<MyDirectory>(), new List<MyFile>());

			// Create the directories if they do not exist
    		CreateDirectory(dirPath);
    		CreateDirectory(outputPath);
    		CreateDirectory(templatePath);
    	}
#endregion

#region Generate Output Methods
    	private static void GenerateOutputDirectorys () {
    		if(!Directory.Exists(outputPath))
    		{
    			Directory.CreateDirectory(outputPath);
    		}
    		GenerateOutputDirectorys (dirPath, outputPath);
    	}

    	private static void GenerateOutputDirectorys (string dir, string odir) {
    		string[] subdirs = Directory.GetDirectories(dir);
    		foreach (string subdir in subdirs) {
    			if (subdir != odir) {
    				string subDirOutput = odir + "\\" + Path.GetFileName (subdir);
    				if (!Directory.Exists (subDirOutput)) {
    					Directory.CreateDirectory (subDirOutput);
    				}
    				GenerateOutputDirectorys (subdir, subDirOutput);
    			}
    		}
    	}

    	private static void GenerateHTMLFiles (string dir, string odir, MyDirectory mydir) {
			// Go through all txt files in this directory
    		string[] files = Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly);
    		foreach (string file in files) {
    			string name_with_ext = Path.GetFileName(file);
    			string name_without_ext = Path.GetFileNameWithoutExtension (file);
    			string outputFile_with_ext = odir + "\\" + name_with_ext;
    			string outputFile_with_HTML_ext = odir + "\\" + name_without_ext + ".html";
				// check it is a txt file
    			if (name_with_ext.Substring (name_with_ext.Length - 4) == ".txt") {
    				StreamWriter writer = new StreamWriter (outputFile_with_HTML_ext, false);
    				writer.Close ();
    				mydir.AddFile(new MyFile(name_without_ext, file, outputFile_with_HTML_ext));
    			} else {
					// copy across all other files (excluding this exe, cs and pdb files which would be this application)
    				string last4chars = name_with_ext.Substring (name_with_ext.Length - 4);
    				if ((last4chars != ".exe") && (last4chars != ".pdb") && (last4chars != ".cs")) {
    					File.Copy (file, outputFile_with_ext);
    				}
    			}
    		}

			// Invoke the same for subdirectories
    		string[] subdirs = Directory.GetDirectories(dir);
    		foreach (string subdir in subdirs) {
    			if (subdir != odir) {
    				string subDirOutput = odir + "\\" + Path.GetFileName (subdir);
    				MyDirectory dirr = new MyDirectory(Path.GetFileName (subdir), subdir, subDirOutput, new List<MyDirectory>(), new List<MyFile>());
    				mydir.AddDirectory(dirr);
    				GenerateHTMLFiles (subdir, subDirOutput, dirr);
    			}
    		}
    	}
#endregion

#region Directory Utility Methods
    	private static void CreateDirectory(string path) {
    		if(!Directory.Exists(path))
    		{
    			Directory.CreateDirectory(path);
    		}
    	}

    	private static void ClearDirectory (string directory) {
    		System.IO.DirectoryInfo di = new DirectoryInfo(directory);
    		foreach (FileInfo file in di.GetFiles())
    		{
    			file.Delete(); 
    		}
    		foreach (DirectoryInfo dir in di.GetDirectories())
    		{
    			dir.Delete(true); 
    		}
    	}
#endregion

#region Template Methods
    	private static string GetTemplateString (string templateName) {
    		string path = templatePath + "\\" + templateName + ".html";
    		StreamReader reader = new StreamReader(path);
    		string contents = reader.ReadToEnd();
    		reader.Close();
    		return contents;
    	}

    	private static string GetTemplateString (string templateName, string[] names, string[] insertText, string filePath) {
    		if (names.Length != insertText.Length) {
    			return "";
    		} else {
    			string content = GetTemplateString(templateName);
    			for (int i = 0; i < names.Length; i++) {
    				content = content.Replace("### " + names[i]  + " ###", insertText[i]);
    			}
    			content = content.Replace("### BASE ITEM NUMBER ###", "" + baseItemNumber);
    			string[] templateFiles = Directory.GetFiles(templatePath, "*.*", SearchOption.AllDirectories);
    			foreach (string s in templateFiles) {
    				string tempPath = s.Substring(templatePath.Length + 1);
    				string relativePath = GetRelativeURIPath(filePath, outputPath + "\\" + tempPath);
    				content = content.Replace("### File " + tempPath.Replace("\\", "/")  + " ###", relativePath.Replace("\\", "/"));
    			}
    			return content;
    		}
    	}

    	private static void CopyTemplateFilesToOutput () {
    		string[] templateFiles = Directory.GetFiles(templatePath, "*.*", SearchOption.AllDirectories);
    		foreach (string s in templateFiles) {
    			if (Array.IndexOf(templateFileNames, s.Substring(templatePath.Length+1)) == -1) {
    				string newFilePath = outputPath + s.Substring(templatePath.Length);
    				CreateDirectory(Path.GetDirectoryName(newFilePath));
    				File.Copy (s, newFilePath);
    			}
    		}

    	}
#endregion

    	public static void DebugTree (List<BaseContent> content) {
    		foreach (BaseContent c in content) {
    			DebugTree(c);
    		}
    	}

    	public static void DebugTree (BaseContent content) {
    		DebugTree(content, 0);
    	}

    	public static void DebugTree (BaseContent content, int indent) {
    		Console.WriteLine(new String(' ', indent * 5) + content.name);
    		if (content is ContentParent) {
    			ContentParent p = (ContentParent)content;
    			foreach (BaseContent c in p.children) {
    				DebugTree(c, indent+1);
    			}
    		}
    	}
    }

    public class MyDirectory {
    	public string dirName;
    	public string inputPath;
    	public string outputPath;
    	public List<MyDirectory> subdirs;
    	public List<MyFile> files;

    	public MyDirectory (string n, string i, string o, List<MyDirectory> s, List<MyFile> f) {
    		dirName = n;
    		inputPath = i;
    		outputPath = o;
    		subdirs = s;
    		files = f;
    	}

    	public void AddFile (MyFile f) {
    		files.Add(f);
    	}

    	public void AddDirectory (MyDirectory d) {
    		subdirs.Add(d);
    	}
    }

    public class MyFile {
    	public string fileName;
    	public string txtPath;
    	public string htmlPath;

    	public MyFile (string n, string i, string o) {
    		fileName = n;
    		txtPath = i;
    		htmlPath = o;
    	}
    }
}

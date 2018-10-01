# Documentation Templater (WIP)

Free Open-Source Mono/.NET Windows/Mac/Linux C# Console application to help you create your HTML documentation using templates.  
  
It comes with an example template using [Bootstrap 4](https://getbootstrap.com/). More templates and features coming soon.

Note: This is project is still WIP. You may encounter bugs. I plan on adding better documentation for using the tool, supporting more page elements, more example templates, and possibly an interface soon.

<img src="https://i.imgur.com/plBgyrW.png" alt="drawing" width="700" height="500" />


## Install

### Windows
Clone the repository using:  
`git clone https://github.com/james231/documentation-templater`  
To generate the documentation you need to execute the `docTemplater.exe`. This is built as a Windows Console Appliction so can be executed by double clicking on through the Command Line.  
If you want to rebuild the application from the source file `Source/docTemplater.cs`, you can use the lates Mono or .NET C# compilers.

### Mac
Clone the repository using:  
`git clone https://github.com/james231/documentation-templater`  
You **cannot** just execute the `docTemplater.exe` because it was built for Windows. Instead you need to rebuild it from the source code youself. You'll need [Mono installed for this](https://www.mono-project.com/download/stable/#download-mac) and you can find comiling information on their website [here](https://www.mono-project.com/docs/compiling-mono/mac/). The source code consists of a single file `Source/docTemplater.cs`.

### Linux
Same story as for Mac.  
Clone the repository using:  
`git clone https://github.com/james231/documentation-templater`  
You **cannot** just execute the `docTemplater.exe` because it was built for Windows. Instead you need to rebuild it from the source code youself. You'll need [Mono installed for this](https://www.mono-project.com/download/stable/#download-lin) and you can find comiling information on their website [here](https://www.mono-project.com/docs/compiling-mono/linux/). The source code consists of a single file `Source/docTemplater.cs`.

## Walkthrough

Note: Don't put anything important in the `\Output` folder. The application will just delete it when executed.

### Template

The first thing you need is a template setup correctly. When you first clone the repository the Bootstrap example template will already be setup in the `\Template` folder. So you can continue to the next section.

Can you create your own templates? Yes, of course you can. It's really simple. I'll be providing more information about this soon.


### Input

Now you need to create some files for your documentation content. There are some example files to help you. If this is your first time then continue to the next step first to build a documentation from the example files.

Each `.txt` file in the `\Input` folder will generate a page in the documentation. The contents of the `.txt` will define the contents on that documentation page.

Take a look at the following markup code used in an example `.txt` file:  
  
```
# Project Name
My Big Project

# Version
1.0.0

# Page Title
Getting Started

## Begin Section

	# Heading
	Nice Big Heading

	# Content
	Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.

## End Section
```

The line `# Project Name` tells the `docTemplater.exe` that the Project Name will be on the next line. In both example templates this project name will be displayed in the top left-hand corner of the page.  
  
Similarily `# Version` is being used to display the version number `1.0.0` somewhere on the page.  
  
Next `# Page Title` gives the page a title.  
  
After this the page content actually starts. On this page we have a single Section. A Section usually consists of a heading followed by some text (unless you leave one of them out). The heading is defined below the `# Heading` line and the text is defined below `# Content`.
  
If you want to add more sections you can add Begin/End Section blocks at the end of the file. If you want to add your own custom single-line HTML you can use `# HTML` with your HTML code on the following line.
  
There are a few other kinds of blocks you can use for adding tables and tabs. I'll provide documentation for them at a later date and I may be adding more elements. For now, the example `.txt` files in the `\Input` folder show all the features available.

### Process

Execute the `docTemplater.exe` if you are on Windows. If you are on Mac, Linux you'll need to build the tool from source using Mono (see above sections).

**IMPORTANT:** Make sure you execute the application in the root directory of the repository, this is exactly where you'll find the `docTemplater.exe` when you clone the repository. **Don't move the file anywhere else** and don't add it to your system PATH.
  
Don't expect any nice messages or customization flags. There's none of that. It's quite basic.

### Output

Look in the `\Output` folder to see the output. You'll see a HTML file for each text file which was inside the `\Input` folder. Subdirectories will have been preserved. The sidenav on each HTML page will match the folder structure and all relative hyperlinks will have been generated correctly. If you have `\css`, `\js` or `\img` folders, these have been copied from the `\Template` folder. They will be in the root of the `\Output` folder, but all HTML files (even in subdirectories) will be linked to them correctly. Also, non-txt files from the `\Input` folder will have been copied over to the `\Output` folder (and it's subdirectories).
  
Each HTML file has main content generated from the markup in the corresponding input text file.

## Quality

The source code is a mess. It's all packed into one file in a meaningless order with no comments. This is because I wrote it within 2 days for another project with a tight deadline. Nobody else was going to see the code until I decided to put it on GitHub. I'll be massively improving the code soon.
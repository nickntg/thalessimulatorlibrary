#############################################################################
Ready to use Doxygen VB documentation generator by Vsevolod Kukol
-----------------------------------------------------------------------------
README.TXT - How To Use
-----------------------------------------------------------------------------
 Creation:     21.06.2010  Vsevolod Kukol
 Last Update:  -

 Copyright (c) 2010 Vsevolod Kukol, sevo(at)sevo(dot)org

 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.
-----------------------------------------------------------------------------

The Doc folder contains all necessary files to create documentation
of VB projects easily.

--------
Contents
--------

Doc\make.bat  - Main script that generates the documentation
Doc\Doxyfile  - Preconfigured Doxygen configuration file
Doc\layout\*  - Default Doxygen layout XML and Stylesheet, that is
                preconfigured in the provided Doxyfile. 
Doc\bin\*     - Folder containing all necessary binary files including
                gawk and the VB filter

------------
Instructions
------------

1. Extract the contents of this package into your project folder. If
   you want to generate the docs for several projects (eg solution) at
   the same time, then extract the package into the main folder (containing
   your projects).
   NOTE: the package contains only one folder called "Doc", so this folder
         should go into the root folder of your project or solution
		 
		 For VB6 the structure could look like:
		 MyProject\MyProject.vbp
		 MyProject\[...]
		 MyProject\Doc\make.bat
		 MyProject\Doc\[...]
		 
		 For .NET the structure could look like:
		 MySolution\MySolution.sln
		 MySolution\[...]
		 MyProject\Doc\make.bat
		 MyProject\Doc\[...]
		 MySolution\MyProject

2. Download and install the Doxygen 1.7.0 (or newer) distribution package
   from the Doxygen download page:
   http://www.stack.nl/~dimitri/doxygen/download.html#latestsrc
   
   or v1.7.1 directly:
   http://ftp.stack.nl/pub/users/dimitri/doxygen-1.7.1-setup.exe
   
3. Optional: Install GraphViz if you want nice graphs in your documentation
			 Link: http://www.graphviz.org/Download_windows.php

4. Open the Doc\Doxyfile configuration file with Doxywizard or any
   text editor (like Notepad++).
   
   If you use Doxywizard, switch to the "Expert" tab to see all options.
   
   Important configuration options:
   
   PROJECT_NAME - set the name of your project here   
   
       HAVE_DOT - Set to YES, to enable Graphs in the documentation.
	              GraphViz (Step 4) has to be installed for this feature.
	
	Do not change the pathes in the file, unless you know what you are
	doing, because it could break the functionality of make.bat!
	
	Read the Doxygen manual for all other options under
	http://www.stack.nl/~dimitri/doxygen/manual.html
	
	NOTE: If you want to run Doxygen directly from inside Doxywizard,
	      set the working folder under:
		  "Step 1: Specify the working directory from which doxygen will run"
		  to the folder CONTAINING the previously extracted Doc folder
		  (in most cases your Project or Solution folder!)
		  
5. run Doc\make.bat to generate the documentation.


------------
Good to know
------------

Integrating in Visual Studio
----------------------------
You can configure Visual Studio to run make.bat each time you build
your project. Or simply create a custom shortcut icon to run make.bat
manually.

Generating CHM (Windows Help)
-----------------------------

If you want Doxygen to generate CHM (Windows Help) files, install
the Microsoft HTML Help Workshop 1.3 (htmlhelp.exe) from
http://go.microsoft.com/fwlink/?LinkId=14188
Direct link: http://go.microsoft.com/fwlink/?LinkId=14188

set the path to hhc.exe using the HHC_LOCATION option
and enable the GENERATE_HTMLHELP option.
(Doxywizard: you'll find both in the HTML section)

GENERATE_HTMLHELP      = YES
HHC_LOCATION           = "C:/Program Files/HTML Help Workshop/hhc.exe"

Other formats
-------------
HTML output is enabled by default. You can additionally enable
RTF, LaTex and some other output formats. LaTex and PDF need
a complete LaText distribution to be installed. Read the
Doxygen manula for more information.

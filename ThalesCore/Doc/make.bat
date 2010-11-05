rem |----------------------------------------------------
rem | MAKE.BAT
rem |-----------------------------------------------------------------------------
rem | Creation:     21.06.2010  Vsevolod Kukol
rem | Last Update:  25.06.2010  Vsevolod Kukol
rem |
rem | Copyright (c) 2010 Vsevolod Kukol, sevo(at)sevo(dot)org
rem |
rem | This program is free software; you can redistribute it and/or modify
rem | it under the terms of the GNU General Public License as published by
rem | the Free Software Foundation; either version 2 of the License, or
rem | (at your option) any later version.
rem |-----------------------------------------------------------------------------

@echo off

rem save current working dir and cd to doc root
pushd %~dp0

rem get the base directory containing make.bat
set DirName=
set DirPath=%~dp0

:loop
  If "%DirPath%" == "" GoTo :loop_done
  For /F "tokens=1* delims=\" %%a in ("%DirPath%") Do set DirName=%%a
  For /F "tokens=1* delims=\" %%a in ("%DirPath%") Do Set DirPath=%%b
  GoTo :loop

:loop_done
rem switch to project root
cd ..

rem run Doxygen
doxygen %DirName%\Doxyfile

rem cd back to original working dir
popd

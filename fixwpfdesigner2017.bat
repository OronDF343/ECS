@echo off
call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat"
cd "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\IDE\PrivateAssemblies"
gacutil -i System.Windows.Interactivity.dll
pause

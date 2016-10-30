@echo off
call "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat"
cd "%ProgramFiles(x86)%\Microsoft SDKs\Expression\Blend\.NETFramework\v4.5\Libraries\"
gacutil -i System.Windows.Interactivity.dll
pause

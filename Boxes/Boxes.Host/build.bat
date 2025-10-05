@echo off
call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
MSBuild Boxes.Host.vcxproj /p:Configuration=Debug /p:Platform=x64
pause

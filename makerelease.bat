@echo off
cd bin
if not exist lib mkdir lib
del /s /f /q lib\*.*
for /f %%f in ('dir /ad /b lib\') do rd /s /q lib\%%f
move x86 lib\x86
move x64 lib\x64
echo y | del x86 && rmdir x86
echo y | del x64 && rmdir x64
for %%i in (*.dll) do move %%~ni.dll lib\%%~ni.dll
for %%i in (*.xml) do del %%~ni.xml
for %%i in (*.pdb) do del %%~ni.pdb
for %%i in (*.config) do if not "%%~ni" == "testcli.exe" del %%~ni.config
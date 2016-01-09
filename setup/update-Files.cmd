set PRODUCTNAME=hagen
set STAGE=%TEMP%\%PRODUCTNAME%-stage
set WIXBIN=%ProgramFiles(x86)%\WiX Toolset v3.8\bin

del /s /q %STAGE%
robocopy /mir /s %~dp0..\..\tta.build\Release-AnyCPU\bin" %STAGE% /xf *.xml /xf *.pdb /xd test
"%WIXBIN%\heat" dir %STAGE% -cg ProductComponents -ag -sreg -sfrag -o %~dp0Files.wxs -var var.BuildDirectory -dr INSTALLFOLDER

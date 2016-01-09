rem %1 : build target. Default: Release

set Target=%1
if "%Target%" == "" (
	set Target=Release
)

set msbuild="%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe"
set SourceDir=%~dp0.
call :file_name_from_path DirName %SourceDir%
%msbuild% %SourceDir%\build\Build.proj /t:StartWrapper /p:BuildTarget=%Target%
goto :eof

:file_name_from_path <resultVar> <pathVar>
(
    set "%~1=%~nx2"
    exit /b
)

:eof

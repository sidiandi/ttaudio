@echo off

rem script to start the build pipeline
rem usage: build [Target]
rem Target: build target. Default: Release. See build/build.targets for available targets

set Target=%1
if "%Target%" == "" (
	set Target=Setup
)

set msbuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
set SourceDir=%~dp0.
call :file_name_from_path DirName %SourceDir%
%msbuild% "%SourceDir%\build\Bootstrap.proj" /p:BuildTarget=%Target% || exit /b 1
goto :eof

:file_name_from_path <resultVar> <pathVar>
(
    set "%~1=%~nx2"
    exit /b
)

:eof

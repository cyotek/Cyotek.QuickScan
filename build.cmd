@ECHO OFF

SETLOCAL

SET SCRIPTPATH=%~dp0
SET SCRIPTPATH=%SCRIPTPATH:~0,-1%

CD %SCRIPTPATH%

CALL %CTKBLDROOT%SetupEnv.cmd

REM Build and sign the file
%msbuildexe% Cyotek.QuickScan.sln /p:Configuration=Release /verbosity:minimal /nologo /t:Clean,Build

IF EXIST dist DEL dist\*.* /q

IF NOT EXIST dist MKDIR dist

PUSHD .\dist

copy /y ..\src\bin\release\ctkqscan.exe
copy /y ..\src\bin\release\ctkqscan.exe.config
copy /y ..\src\bin\release\ctkqscan.pdb
copy /y ..\src\bin\release\ctkqscan.default.ini
copy /y ..\src\bin\release\Cyotek.Windows.Forms.ImageBox.dll
copy /y ..\src\bin\release\Cyotek.Data.Ini.dll
copy /y ..\src\bin\release\LICENSE.txt
copy /y ..\src\bin\release\README.md

CALL sign-program ctkqscan.exe

%zipexe% a Cyotek.QuickScan.1.0.x.zip -r

POPD

ENDLOCAL

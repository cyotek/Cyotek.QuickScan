@ECHO OFF

SETLOCAL

SET SCRIPTPATH=%~dp0
SET SCRIPTPATH=%SCRIPTPATH:~0,-1%

CD %SCRIPTPATH%

CALL %CTKBLDROOT%\SetupEnv.cmd

REM Build and sign the file
%msbuildexe% Cyotek.QuickScan.sln /p:Configuration=Release /verbosity:minimal /nologo /t:Clean,Build

IF EXIST dist         DEL dist\*.* /q
IF EXIST dist\sounds  DEL dist\sounds\*.* /q

IF NOT EXIST dist MKDIR dist
IF NOT EXIST dist\sounds MKDIR dist\sounds

PUSHD .\dist

copy /y ..\src\bin\release\ctkqscan.exe
copy /y ..\src\bin\release\ctkqscan.exe.config
copy /y ..\src\bin\release\ctkqscan.pdb
copy /y ..\src\bin\release\ctkqscan.default.ini
copy /y ..\src\bin\release\Cyotek.Windows.Forms.ImageBox.dll
copy /y ..\src\bin\release\Cyotek.Data.Ini.dll
copy /y ..\LICENSE.txt
copy /y ..\README.md
copy /y ..\CHANGELOG.md
copy /y ..\res\gmae.wav sounds\
copy /y ..\restartservice\bin\release\rstrtwia.exe

CALL sign-program ctkqscan.exe
CALL sign-program rstrtwia.exe

%zipexe% a Cyotek.QuickScan.1.0.x.zip -r

POPD

ENDLOCAL

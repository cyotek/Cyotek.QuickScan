@ECHO OFF

SETLOCAL

SET SCRIPTPATH=%~dp0
SET SCRIPTPATH=%SCRIPTPATH:~0,-1%
SET RELDIR=%SCRIPTPATH%\src\bin\release\net48

CD %SCRIPTPATH%

CALL %CTKBLDROOT%\SetupEnv.cmd

REM Build and sign the file
%msbuildexe% Cyotek.QuickScan.slnx /p:Configuration=Release /verbosity:minimal /nologo /t:Clean,Build

IF EXIST dist         DEL dist\*.* /q
IF EXIST dist\sounds  DEL dist\sounds\*.* /q

IF NOT EXIST dist MKDIR dist
IF NOT EXIST dist\sounds MKDIR dist\sounds

PUSHD .\dist

copy /y %RELDIR%\ctkqscan.exe
copy /y %RELDIR%\ctkqscan.exe.config
copy /y %RELDIR%\ctkqscan.pdb
copy /y %RELDIR%\ctkqscan.default.ini
copy /y %RELDIR%\Cyotek.Windows.Forms.ImageBox.dll
copy /y %RELDIR%\Cyotek.Data.Ini.dll
copy /y ..\LICENSE.txt
copy /y ..\README.md
copy /y ..\CHANGELOG.md
copy /y ..\res\gmae.wav sounds\
copy /y ..\restartservice\bin\release\net48\rstrtwia.exe

CALL sign-program ctkqscan.exe
CALL sign-program rstrtwia.exe

%zipexe% a Cyotek.QuickScan.1.0.x.zip -r

POPD

ENDLOCAL

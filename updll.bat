@echo off
openfiles >nul 2>&1
if %errorlevel% neq 0 (
echo. Administrator privileges are required. Please run this file as an administrator.
pause
exit /b
)

goto check_para

:usage_short
	echo.  UPDLL : Copy updated "Include/SDOAQ/*.dll, Include/WSIO/*.dll" files to "C:/Windows/System32".
	goto exit

:usage
	call updll.bat #
	echo.
	echo.  "UPDLL"      : run one time.
	echo.  "UPDLL LOOP" : run loop mode. Press Ctrl+C to finish.
	goto exit

:check_para
	if "%1"=="#" goto usage_short
	if "%1"=="?" goto usage
	if "%1"=="loop" goto loop
	if "%1"=="LOOP" goto loop
	goto main

:main
	echo.
	copy %~dp0\Include\SDOAQ\*.dll C:\Windows\System32 /D/Y
	copy %~dp0\Include\WSIO\*.dll C:\Windows\System32 /D/Y
	pause
	goto exit

:loop
	call updll.bat
	call updll.bat loop
	goto exit

:exit

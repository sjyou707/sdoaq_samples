@echo off
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
	time < D:\MySystem\BIN\sub\enter.txt | find "."
	echo.
	xcopy Include\SDOAQ\*.dll C:\Windows\System32 /D/Y
	xcopy Include\WSIO\*.dll C:\Windows\System32 /D/Y
	echo.    press any key to continue...
	pause > D:\MySystem\BIN\sub\null.txt

	goto exit

:loop
	call updll.bat
	call updll.bat loop
	goto exit

:exit

@echo off

REM Check for admin privileges
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting administrator privileges...
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

echo Stopping all Bloodstrap processes...

REM Kill all Bloodstrap.exe processes
taskkill /F /IM Bloodstrap.exe /T 2>nul
if %errorlevel% equ 0 (
    echo Successfully stopped Bloodstrap.exe
) else (
    echo No Bloodstrap.exe processes found
)

REM Kill any Roblox processes that might be associated
taskkill /F /IM RobloxPlayerBeta.exe /T 2>nul
if %errorlevel% equ 0 (
    echo Successfully stopped RobloxPlayerBeta.exe
)

taskkill /F /IM RobloxStudioBeta.exe /T 2>nul
if %errorlevel% equ 0 (
    echo Successfully stopped RobloxStudioBeta.exe
)

taskkill /F /IM bootstrapper.exe /T 2>nul
if %errorlevel% equ 0 (
    echo Successfully stopped bootstrapper.exe
)

echo.
echo All processes stopped.
pause

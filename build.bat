@echo off
echo ========================================
echo   Voidstrap Build Script
echo ========================================
echo.

REM Check if dotnet is available
where dotnet >nul 2>nul
if %ERRORLEVEL% EQU 0 (
    echo Using .NET SDK to build...
    echo.
    dotnet build Bloxstrap\Voidstrap.csproj -c Release
    if %ERRORLEVEL% EQU 0 (
        echo.
        echo ========================================
        echo   Build Successful!
        echo ========================================
        echo.
        echo Opening output folder...
        start explorer.exe "Bloxstrap\bin\Release"
    ) else (
        echo.
        echo ========================================
        echo   Build Failed!
        echo ========================================
        pause
        exit /b 1
    )
) else (
    echo .NET SDK not found. Trying PowerShell build script...
    echo.
    powershell -ExecutionPolicy Bypass -File "%~dp0build.ps1"
)

echo.
pause

@echo off
REM Load project configuration
call project-config.bat

echo ========================================
echo   %PROJECT_NAME% Data Reset Tool
echo ========================================
echo.
echo WARNING: This will DELETE ALL %PROJECT_NAME% data including:
echo   - Settings and configuration
echo   - FastFlags
echo   - Mods and customizations
echo   - Roblox cookies
echo   - Activity history
echo   - Shortcuts
echo   - All saved data
echo.
echo This action CANNOT be undone!
echo.
pause
echo.

echo Closing %PROJECT_NAME% if running...
taskkill /F /IM %PROJECT_NAME%.exe 2>nul
timeout /t 2 /nobreak >nul

echo.
echo [1/8] Removing %PROJECT_NAME% AppData...
if exist "%LocalAppData%\%PROJECT_NAME%" (
    rmdir /S /Q "%LocalAppData%\%PROJECT_NAME%"
    echo   ✓ Deleted: %LocalAppData%\%PROJECT_NAME%
) else (
    echo   - Not found: %LocalAppData%\%PROJECT_NAME%
)

if exist "%LocalAppData%\%OLD_PROJECT_NAME%" (
    rmdir /S /Q "%LocalAppData%\%OLD_PROJECT_NAME%"
    echo   ✓ Deleted: %LocalAppData%\%OLD_PROJECT_NAME%
) else (
    echo   - Not found: %LocalAppData%\%OLD_PROJECT_NAME%
)

echo.
echo [2/8] Removing Roblox cookies...
if exist "%LocalAppData%\Roblox\LocalStorage\RobloxCookies.dat" (
    del /F /Q "%LocalAppData%\Roblox\LocalStorage\RobloxCookies.dat"
    echo   ✓ Deleted: RobloxCookies.dat
) else (
    echo   - Not found: RobloxCookies.dat
)

echo.
echo [3/8] Removing registry entries...
reg delete "HKCU\Software\%PROJECT_NAME%" /f 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   ✓ Deleted: HKCU\Software\%PROJECT_NAME%
) else (
    echo   - Not found: HKCU\Software\%PROJECT_NAME%
)

reg delete "HKCU\Software\%OLD_PROJECT_NAME%" /f 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   ✓ Deleted: HKCU\Software\%OLD_PROJECT_NAME%
) else (
    echo   - Not found: HKCU\Software\%OLD_PROJECT_NAME%
)

reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\%PROJECT_NAME%" /f 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   ✓ Deleted: Uninstall registry key
) else (
    echo   - Not found: Uninstall registry key
)

echo.
echo [4/8] Removing desktop shortcuts...
if exist "%UserProfile%\Desktop\%PROJECT_NAME%*.lnk" (
    del /F /Q "%UserProfile%\Desktop\%PROJECT_NAME%*.lnk"
    echo   ✓ Deleted: Desktop shortcuts
) else (
    echo   - Not found: Desktop shortcuts
)

if exist "%UserProfile%\Desktop\%OLD_PROJECT_NAME%*.lnk" (
    del /F /Q "%UserProfile%\Desktop\%OLD_PROJECT_NAME%*.lnk"
    echo   ✓ Deleted: Old desktop shortcuts
) else (
    echo   - Not found: Old desktop shortcuts
)

echo.
echo [5/8] Removing Start Menu shortcuts...
if exist "%AppData%\Microsoft\Windows\Start Menu\Programs\%PROJECT_NAME%" (
    rmdir /S /Q "%AppData%\Microsoft\Windows\Start Menu\Programs\%PROJECT_NAME%"
    echo   ✓ Deleted: Start Menu folder
) else (
    echo   - Not found: Start Menu folder
)

if exist "%AppData%\Microsoft\Windows\Start Menu\Programs\%OLD_PROJECT_NAME%" (
    rmdir /S /Q "%AppData%\Microsoft\Windows\Start Menu\Programs\%OLD_PROJECT_NAME%"
    echo   ✓ Deleted: Old Start Menu folder
) else (
    echo   - Not found: Old Start Menu folder
)

echo.
echo [6/8] Removing temporary files...
if exist "%Temp%\%PROJECT_NAME%*" (
    del /F /Q "%Temp%\%PROJECT_NAME%*"
    echo   ✓ Deleted: Temp files
) else (
    echo   - Not found: Temp files
)

echo.
echo [7/8] Removing Roblox modifications...
if exist "%LocalAppData%\Roblox\Versions" (
    for /d %%D in ("%LocalAppData%\Roblox\Versions\*") do (
        if exist "%%D\content\fonts" (
            rmdir /S /Q "%%D\content\fonts" 2>nul
        )
        if exist "%%D\content\sounds" (
            rmdir /S /Q "%%D\content\sounds" 2>nul
        )
        if exist "%%D\content\textures" (
            rmdir /S /Q "%%D\content\textures" 2>nul
        )
    )
    echo   ✓ Cleaned: Roblox modifications
) else (
    echo   - Not found: Roblox versions
)

echo.
echo [8/8] Removing configuration backups...
if exist "%UserProfile%\Documents\%PROJECT_NAME% Backups" (
    rmdir /S /Q "%UserProfile%\Documents\%PROJECT_NAME% Backups"
    echo   ✓ Deleted: Configuration backups
) else (
    echo   - Not found: Configuration backups
)

echo.
echo ========================================
echo   Data Reset Complete!
echo ========================================
echo.
echo All %PROJECT_NAME% data has been removed.
echo The application will start fresh on next run.
echo.
echo Note: Roblox itself was NOT uninstalled.
echo To reinstall %PROJECT_NAME%, run %PROJECT_NAME%.exe again.
echo.
pause

@echo off
echo ========================================
echo   Bloodstrap Single-File Builder
echo ========================================
echo.

echo Building single-file executable...
echo.

cd Bloxstrap

dotnet publish Voidstrap.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   Build Successful!
    echo ========================================
    echo.
    echo Single-file executable created at:
    echo bin\Release\net8.0-windows7.0\win-x64\publish\Voidstrap.exe
    echo.
    echo This is a single .exe file that contains everything!
    echo You can distribute just this one file.
    echo.
    
    REM Open the publish folder
    start "" "bin\Release\net8.0-windows7.0\win-x64\publish"
) else (
    echo.
    echo ========================================
    echo   Build Failed!
    echo ========================================
    echo.
)

pause

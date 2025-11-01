@echo off
REM Load project configuration
call project-config.bat

echo ========================================
echo   Final %PROJECT_NAME% Rebrand
echo ========================================
echo.

cd Bloxstrap

echo Updating ALL XAML Title and Text attributes...
powershell -Command "$oldName='%OLD_PROJECT_NAME%'; $newName='%PROJECT_NAME%'; $files = Get-ChildItem -Recurse -Filter *.xaml; foreach ($file in $files) { $content = [System.IO.File]::ReadAllText($file.FullName); $original = $content; $content = $content -replace \"Title=`\"$oldName`\"\", \"Title=`\"$newName`\"\"; $content = $content -replace \"Title=`\"$oldName Installer`\"\", \"Title=`\"$newName Installer`\"\"; $content = $content -replace \">$oldName<\", \">$newName<\"; $content = $content -replace \"Text=`\"$oldName\", \"Text=`\"$newName\"; $content = $content -replace \"`\"$oldName v\", \"`\"$newName v\"; if ($content -ne $original) { [System.IO.File]::WriteAllText($file.FullName, $content); Write-Host `"  ✓ $($file.Name)`" -ForegroundColor Green } }"

cd ..

echo.
echo Building Bloodstrap.exe...
echo.
dotnet clean Bloxstrap\Voidstrap.csproj -c Release
dotnet publish Bloxstrap\Voidstrap.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   SUCCESS!
    echo ========================================
    echo.
    explorer Bloxstrap\bin\Release\net8.0-windows7.0\win-x64\publish
) else (
    echo.
    echo Build failed!
)

pause

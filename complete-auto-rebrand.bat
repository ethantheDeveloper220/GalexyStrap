@echo off
echo ========================================
echo   COMPLETE Automatic Rebrand Tool
echo ========================================
echo.
echo This will rebrand EVERYTHING in the application.
echo.

REM ========================================
REM CONFIGURATION - CHANGE THESE VALUES
REM ========================================

SET NEW_PROJECT_NAME=Bloodstrap
SET NEW_PROJECT_OWNER=ethantheDeveloper220
SET NEW_PROJECT_REPO=vibetrap

SET OLD_PROJECT_NAME=Voidstrap

REM ========================================
REM DO NOT EDIT BELOW THIS LINE
REM ========================================

echo Current Configuration:
echo   Project Name: %NEW_PROJECT_NAME%
echo   Owner: %NEW_PROJECT_OWNER%
echo   Repository: %NEW_PROJECT_REPO%
echo.
pause
echo.

echo [1/6] Updating ProjectInfo.cs...
powershell -Command "$file='Bloxstrap\ProjectInfo.cs'; $content=[System.IO.File]::ReadAllText($file); $content=$content -replace 'PROJECT_NAME = \".*?\"', 'PROJECT_NAME = \"%NEW_PROJECT_NAME%\"'; $content=$content -replace 'PROJECT_OWNER = \".*?\"', 'PROJECT_OWNER = \"%NEW_PROJECT_OWNER%\"'; $content=$content -replace 'PROJECT_REPO = \".*?\"', 'PROJECT_REPO = \"%NEW_PROJECT_REPO%\"'; [System.IO.File]::WriteAllText($file, $content); Write-Host '  ✓ Updated ProjectInfo.cs' -ForegroundColor Green"

echo.
echo [2/6] Updating project-config.bat...
powershell -Command "$file='project-config.bat'; $content=[System.IO.File]::ReadAllText($file); $content=$content -replace 'SET PROJECT_NAME=.*', 'SET PROJECT_NAME=%NEW_PROJECT_NAME%'; $content=$content -replace 'SET OLD_PROJECT_NAME=.*', 'SET OLD_PROJECT_NAME=%OLD_PROJECT_NAME%'; [System.IO.File]::WriteAllText($file, $content); Write-Host '  ✓ Updated project-config.bat' -ForegroundColor Green"

echo.
echo [3/6] Updating ALL XAML files (titles, text, content)...
cd Bloxstrap
powershell -Command "$oldName='%OLD_PROJECT_NAME%'; $newName='%NEW_PROJECT_NAME%'; $files = Get-ChildItem -Recurse -Filter *.xaml; $count=0; foreach ($file in $files) { $content = [System.IO.File]::ReadAllText($file.FullName); $original = $content; $content = $content -replace \"Title=`\"$oldName`\"\", \"Title=`\"$newName`\"\"; $content = $content -replace \"Title=`\"$oldName Installer`\"\", \"Title=`\"$newName Installer`\"\"; $content = $content -replace \"Title=`\"$oldName Settings`\"\", \"Title=`\"$newName Settings`\"\"; $content = $content -replace \">$oldName<\", \">$newName<\"; $content = $content -replace \"Text=`\"$oldName\", \"Text=`\"$newName\"; $content = $content -replace \"`\"$oldName v\", \"`\"$newName v\"; $content = $content -replace \"$oldName Configuration\", \"$newName Configuration\"; $content = $content -replace \"$oldName News\", \"$newName News\"; $content = $content -replace \"$oldName Settings\", \"$newName Settings\"; $content = $content -replace \"Header=`\"$oldName\", \"Header=`\"$newName\"; if ($content -ne $original) { [System.IO.File]::WriteAllText($file.FullName, $content); $count++; Write-Host \"  ✓ $($file.Name)\" -ForegroundColor Green } }; Write-Host \"  Updated $count files\" -ForegroundColor Cyan"
cd ..

echo.
echo [4/6] Updating ALL C# files (strings, comments, text)...
cd Bloxstrap
powershell -Command "$oldName='%OLD_PROJECT_NAME%'; $newName='%NEW_PROJECT_NAME%'; $files = Get-ChildItem -Recurse -Filter *.cs -Exclude *.Designer.cs,*.g.cs; $count=0; foreach ($file in $files) { $content = [System.IO.File]::ReadAllText($file.FullName); $original = $content; $content = $content -replace \"`\"$oldName`\"\", \"`\"$newName`\"\"; $content = $content -replace \"$oldName v\", \"$newName v\"; $content = $content -replace \"$oldName Settings\", \"$newName Settings\"; $content = $content -replace \"$oldName News\", \"$newName News\"; $content = $content -replace \"// $oldName\", \"// $newName\"; if ($content -ne $original) { [System.IO.File]::WriteAllText($file.FullName, $content); $count++; Write-Host \"  ✓ $($file.Name)\" -ForegroundColor Green } }; Write-Host \"  Updated $count files\" -ForegroundColor Cyan"
cd ..

echo.
echo [5/6] Updating resource strings (Strings.resx)...
powershell -Command "$file='Bloxstrap\Resources\Strings.resx'; if (Test-Path $file) { $content=[System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8); $original=$content; $content=$content -replace '>%OLD_PROJECT_NAME%<', '>%NEW_PROJECT_NAME%<'; $content=$content -replace '%OLD_PROJECT_NAME% v', '%NEW_PROJECT_NAME% v'; $content=$content -replace '%OLD_PROJECT_NAME% was', '%NEW_PROJECT_NAME% was'; $content=$content -replace '%OLD_PROJECT_NAME% does', '%NEW_PROJECT_NAME% does'; $content=$content -replace '%OLD_PROJECT_NAME% modifications', '%NEW_PROJECT_NAME% modifications'; $content=$content -replace 'Getting the latest %OLD_PROJECT_NAME%', 'Getting the latest %NEW_PROJECT_NAME%'; if ($content -ne $original) { [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8); Write-Host '  ✓ Updated Strings.resx' -ForegroundColor Green } else { Write-Host '  - No changes needed' -ForegroundColor Yellow } }"

echo.
echo [6/6] Building %NEW_PROJECT_NAME%.exe...
echo.
dotnet clean Bloxstrap\Voidstrap.csproj -c Release
dotnet publish Bloxstrap\Voidstrap.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   COMPLETE REBRAND SUCCESS!
    echo ========================================
    echo.
    echo Your application has been FULLY rebranded to: %NEW_PROJECT_NAME%
    echo.
    echo All files updated:
    echo   ✓ ProjectInfo.cs
    echo   ✓ All XAML files (titles, headers, text)
    echo   ✓ All C# files (strings, comments)
    echo   ✓ Resource strings
    echo   ✓ Configuration files
    echo.
    echo Output: Bloxstrap\bin\Release\net8.0-windows7.0\win-x64\publish\%NEW_PROJECT_NAME%.exe
    echo.
    explorer Bloxstrap\bin\Release\net8.0-windows7.0\win-x64\publish
) else (
    echo.
    echo ========================================
    echo   Build Failed!
    echo ========================================
    echo Please check the errors above.
)

echo.
pause

@echo off
echo ========================================
echo   Updating All Icons to Bloodstrap
echo ========================================
echo.

cd Bloxstrap\UI

echo Updating XAML files...
powershell -Command "(Get-Content -Path '**\*.xaml' -Raw) -replace 'pack://application:,,,/Voidstrap.ico', 'pack://application:,,,/Bloodstrap.ico' | Set-Content -Path '**\*.xaml'"

echo.
echo Updating C# files...
cd ..
powershell -Command "(Get-Content -Path 'UI\NotifyIconWrapper.cs' -Raw) -replace 'IconVoidstrap', 'IconBloodstrap' | Set-Content -Path 'UI\NotifyIconWrapper.cs'"
powershell -Command "(Get-Content -Path 'UI\Frontend.cs' -Raw) -replace 'IconVoidstrap', 'IconBloodstrap' | Set-Content -Path 'UI\Frontend.cs'"
powershell -Command "(Get-Content -Path 'UI\ViewModels\Settings\AppearanceViewModel.cs' -Raw) -replace 'IconVoidstrap', 'IconBloodstrap' | Set-Content -Path 'UI\ViewModels\Settings\AppearanceViewModel.cs'"

echo.
echo ========================================
echo   Icon Update Complete!
echo ========================================
echo.
echo All references updated to Bloodstrap.ico
echo.

pause

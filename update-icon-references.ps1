# PowerShell script to update all icon references to Bloodstrap

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Bloodstrap Icon Reference Updater" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$icoPath = "Bloxstrap\Bloodstrap.ico"

# Check if ICO file exists
if (-not (Test-Path $icoPath)) {
    Write-Host "✗ Error: Bloodstrap.ico not found!" -ForegroundColor Red
    Write-Host "  Please create the ICO file first using convert-icon.ps1" -ForegroundColor Yellow
    Write-Host ""
    pause
    exit
}

Write-Host "✓ Found: $icoPath" -ForegroundColor Green
Write-Host ""

# Copy to Resources folder
Write-Host "Copying icon to Resources folder..." -ForegroundColor Cyan
$resourcesPath = "Bloxstrap\Resources\IconBloodstrap.ico"
Copy-Item $icoPath $resourcesPath -Force
Write-Host "✓ Created: $resourcesPath" -ForegroundColor Green
Write-Host ""

# Update .csproj file
Write-Host "Updating Voidstrap.csproj..." -ForegroundColor Cyan
$csprojPath = "Bloxstrap\Voidstrap.csproj"
$csprojContent = Get-Content $csprojPath -Raw

# Replace icon references
$csprojContent = $csprojContent -replace 'Voidstrap\.ico', 'Bloodstrap.ico'
$csprojContent = $csprojContent -replace 'IconVoidstrap\.ico', 'IconBloodstrap.ico'

Set-Content $csprojPath $csprojContent -NoNewline
Write-Host "✓ Updated: $csprojPath" -ForegroundColor Green
Write-Host ""

# Create PNG version for resources
Write-Host "Note: You may also want to create Bloodstrap.png (256x256)" -ForegroundColor Yellow
Write-Host "  Use the same design as the ICO file" -ForegroundColor Gray
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Icon Update Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Files updated:" -ForegroundColor White
Write-Host "  ✓ Bloxstrap\Bloodstrap.ico" -ForegroundColor Gray
Write-Host "  ✓ Bloxstrap\Resources\IconBloodstrap.ico" -ForegroundColor Gray
Write-Host "  ✓ Bloxstrap\Voidstrap.csproj" -ForegroundColor Gray
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Rebuild the project" -ForegroundColor White
Write-Host "  2. The new Bloodstrap icon will be used!" -ForegroundColor White
Write-Host ""

pause

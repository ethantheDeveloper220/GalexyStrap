# PowerShell script to convert SVG to ICO
# Requires ImageMagick or uses online conversion

$svgPath = "Bloxstrap\Resources\BloodstrapIcon.svg"
$icoPath = "Bloxstrap\Bloodstrap.ico"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Bloodstrap Icon Converter" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if ImageMagick is installed
$magickInstalled = Get-Command magick -ErrorAction SilentlyContinue

if ($magickInstalled) {
    Write-Host "ImageMagick found! Converting SVG to ICO..." -ForegroundColor Green
    Write-Host ""
    
    # Convert SVG to multiple sizes and combine into ICO
    magick convert $svgPath -background none -define icon:auto-resize=256,128,64,48,32,16 $icoPath
    
    if (Test-Path $icoPath) {
        Write-Host "Success! Created: $icoPath" -ForegroundColor Green
        Write-Host ""
        Write-Host "Icon sizes included: 256, 128, 64, 48, 32, 16" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Opening folder..." -ForegroundColor Cyan
        Start-Process explorer.exe -ArgumentList "/select,$PWD\$icoPath"
    } else {
        Write-Host "Failed to create ICO file" -ForegroundColor Red
    }
} else {
    Write-Host "ImageMagick not found!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please use one of these methods to convert:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Option 1: Install ImageMagick" -ForegroundColor White
    Write-Host "  Download from: https://imagemagick.org/script/download.php" -ForegroundColor Gray
    Write-Host "  Then run this script again" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 2: Online Converter (Easiest)" -ForegroundColor White
    Write-Host "  1. Go to: https://convertio.co/svg-ico/" -ForegroundColor Gray
    Write-Host "  2. Upload: $svgPath" -ForegroundColor Gray
    Write-Host "  3. Download as: Bloodstrap.ico" -ForegroundColor Gray
    Write-Host "  4. Place in: Bloxstrap\" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 3: Use Photopea" -ForegroundColor White
    Write-Host "  1. Go to: https://www.photopea.com/" -ForegroundColor Gray
    Write-Host "  2. File -> Open -> Select $svgPath" -ForegroundColor Gray
    Write-Host "  3. File -> Export As -> ICO" -ForegroundColor Gray
    Write-Host "  4. Save as: Bloodstrap.ico in Bloxstrap\" -ForegroundColor Gray
    Write-Host ""
    
    # Open the SVG file location
    Write-Host "Opening SVG file location..." -ForegroundColor Cyan
    Start-Process explorer.exe -ArgumentList "/select,$PWD\$svgPath"
}

Write-Host ""
Write-Host "After creating the ICO file, run update-icon-references.ps1" -ForegroundColor Yellow
Write-Host ""

pause

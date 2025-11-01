$file = "Bloxstrap\Resources\Strings.resx"
$content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
$content = $content -replace 'Voidstrap', 'Bloodstrap'
[System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
Write-Host "Updated Strings.resx - replaced all Voidstrap with Bloodstrap" -ForegroundColor Green

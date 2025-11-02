# Replace all Bloodstrap references with GalaxyStrap
Write-Host "Replacing Bloodstrap with GalaxyStrap..." -ForegroundColor Cyan

$files = Get-ChildItem -Path '.\Bloxstrap' -Recurse -Include *.cs,*.xaml,*.resx

$count = 0
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    if ($content -match 'Bloodstrap') {
        $newContent = $content -replace 'Bloodstrap', 'GalaxyStrap'
        $newContent = $newContent -replace 'bloodstrap', 'galaxystrap'
        Set-Content $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
        Write-Host "Updated: $($file.Name)" -ForegroundColor Green
        $count++
    }
}

Write-Host ""
Write-Host "Updated $count files" -ForegroundColor Cyan
Write-Host "Done!" -ForegroundColor Green

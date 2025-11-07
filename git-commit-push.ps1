# Git Add, Commit, and Push Script
# This script adds all changes, commits with a custom message, and pushes to remote

Write-Host "=== Git Add, Commit, and Push ===" -ForegroundColor Cyan
Write-Host ""

# Check if git is available
try {
    $gitPath = "C:\Program Files\Git\bin\git.exe"
    if (-not (Test-Path $gitPath)) {
        $gitPath = "git"
    }
    & $gitPath --version | Out-Null
} catch {
    Write-Host "Error: Git is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Show current status
Write-Host "Current Git Status:" -ForegroundColor Yellow
& $gitPath status --short

Write-Host ""
Write-Host "Adding all files..." -ForegroundColor Green
& $gitPath add .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to add files" -ForegroundColor Red
    exit 1
}

Write-Host "Files added successfully!" -ForegroundColor Green
Write-Host ""

# Get commit message from user
$commitMessage = Read-Host "Enter commit message"

if ([string]::IsNullOrWhiteSpace($commitMessage)) {
    Write-Host "Error: Commit message cannot be empty" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Committing changes..." -ForegroundColor Green
& $gitPath commit -m "$commitMessage"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to commit changes" -ForegroundColor Red
    exit 1
}

Write-Host "Changes committed successfully!" -ForegroundColor Green
Write-Host ""

# Check if remote exists
$remotes = & $gitPath remote
if ([string]::IsNullOrWhiteSpace($remotes)) {
    Write-Host "Warning: No remote repository configured" -ForegroundColor Yellow
    $addRemote = Read-Host "Do you want to add a remote repository? (y/n)"
    
    if ($addRemote -eq "y" -or $addRemote -eq "Y") {
        $remoteUrl = Read-Host "Enter remote repository URL (e.g., https://github.com/user/repo.git)"
        if (-not [string]::IsNullOrWhiteSpace($remoteUrl)) {
            & $gitPath remote add origin $remoteUrl
            Write-Host "Remote 'origin' added successfully!" -ForegroundColor Green
        } else {
            Write-Host "No remote URL provided. Skipping push." -ForegroundColor Yellow
            exit 0
        }
    } else {
        Write-Host "Skipping push (no remote configured)" -ForegroundColor Yellow
        exit 0
    }
}

# Get current branch
$currentBranch = & $gitPath branch --show-current

Write-Host "Pushing to remote repository (branch: $currentBranch)..." -ForegroundColor Green
& $gitPath push -u origin $currentBranch

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to push changes" -ForegroundColor Red
    Write-Host "You may need to set up authentication or check your remote URL" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "=== Success! ===" -ForegroundColor Green
Write-Host "All changes have been added, committed, and pushed!" -ForegroundColor Green

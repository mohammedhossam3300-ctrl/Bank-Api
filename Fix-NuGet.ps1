# Script to fix NuGet fallback package folder errors
# Run this if you encounter: "Unable to find fallback package folder 'C:\Program Files (x86)\Microsoft Visual Studio\Shared\NuGetPackages'"

Write-Host "=== NuGet Cache Fix ===" -ForegroundColor Green
Write-Host "This will clear Visual Studio cache and NuGet cache" -ForegroundColor Yellow
Write-Host ""

$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path

# Stop Visual Studio if running
Write-Host "Step 1: Closing Visual Studio..." -ForegroundColor Cyan
Stop-Process -Name "devenv" -ErrorAction SilentlyContinue -Force
Start-Sleep -Seconds 2

# Remove .vs folders
Write-Host "Step 2: Removing .vs folders..." -ForegroundColor Cyan
Remove-Item -Path "$rootPath\.vs" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$rootPath\src\.vs" -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "✓ .vs folders removed"

# Clear bin/obj folders
Write-Host "Step 3: Removing bin/obj folders..." -ForegroundColor Cyan
Get-ChildItem -Path "$rootPath\src" -Directory -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.Name -in @('bin', 'obj') } | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "✓ bin/obj folders removed"

# Clear NuGet caches
Write-Host "Step 4: Clearing NuGet caches..." -ForegroundColor Cyan
$nugetCache = "$env:LocalAppData\NuGet\v3-cache"
$nugetScratch = "$env:LocalAppData\Temp\NuGetScratch"

if (Test-Path $nugetCache) {
    Remove-Item -Path $nugetCache -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "✓ NuGet v3-cache cleared"
}
if (Test-Path $nugetScratch) {
    Remove-Item -Path $nugetScratch -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "✓ NuGet scratch cleared"
}

# Clear global packages
Write-Host "Step 5: Clearing global NuGet packages..." -ForegroundColor Cyan
& dotnet nuget locals all --clear 2>&1 | Where-Object { $_ -match "cleared|Clearing" } | ForEach-Object { Write-Host "  $_" }

Write-Host ""
Write-Host "=== Fix Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. cd src"
Write-Host "  2. dotnet restore"
Write-Host "  3. dotnet build"
Write-Host "  4. dotnet publish -c Release -o ./publish"
Write-Host ""

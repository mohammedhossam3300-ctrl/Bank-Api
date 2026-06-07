# Comprehensive build script that fixes all NuGet issues
# Run: .\build.ps1

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Green
Write-Host "BANK-API BUILD SCRIPT" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""

# Step 1: Kill Visual Studio
Write-Host "Step 1: Closing Visual Studio..." -ForegroundColor Cyan
Stop-Process -Name "devenv" -ErrorAction SilentlyContinue -Force
Start-Sleep -Seconds 3

# Step 2: Set environment variables
Write-Host "Step 2: Setting environment variables..." -ForegroundColor Cyan
$env:NUGET_FALLBACK_PACKAGES = ""
$env:NUGET_PACKAGES = "$env:USERPROFILE\.nuget\packages"
$env:NUGET_NO_CACHE = "true"
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "true"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "true"
Write-Host "✓ Environment variables set" -ForegroundColor Green

# Step 3: Clean
Write-Host "Step 3: Cleaning project..." -ForegroundColor Cyan
Remove-Item -Path ".\.vs" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "src\.vs" -Recurse -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path "src" -Directory -Recurse -ErrorAction SilentlyContinue | `
    Where-Object { $_.Name -in @('bin', 'obj') } | `
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "✓ Project cleaned" -ForegroundColor Green

# Step 4: Clear NuGet caches
Write-Host "Step 4: Clearing NuGet caches..." -ForegroundColor Cyan
& dotnet nuget locals all --clear
Write-Host "✓ Caches cleared" -ForegroundColor Green

# Step 5: Restore packages
Write-Host "Step 5: Restoring NuGet packages..." -ForegroundColor Cyan
Push-Location "src"
dotnet restore --no-cache
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Restore failed!" -ForegroundColor Red
    exit 1
}
Pop-Location
Write-Host "✓ Packages restored" -ForegroundColor Green

# Step 6: Build
Write-Host "Step 6: Building solution..." -ForegroundColor Cyan
Push-Location "src"
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
Pop-Location
Write-Host "✓ Build successful" -ForegroundColor Green

# Step 7: Publish
Write-Host "Step 7: Publishing release..." -ForegroundColor Cyan
Push-Location "src"
dotnet publish -c Release -o "../publish"
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Publish failed!" -ForegroundColor Red
    exit 1
}
Pop-Location
Write-Host "✓ Publish successful" -ForegroundColor Green

Write-Host ""
Write-Host "======================================" -ForegroundColor Green
Write-Host "✅ BUILD COMPLETE" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "Output location: ./publish" -ForegroundColor Cyan
Write-Host ""


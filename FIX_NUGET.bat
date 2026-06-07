@echo off
REM This script fixes the NuGet fallback package folder error
REM Run this as Administrator

echo Removing .vs folders and cache...
cd /d "c:\Users\Memo\Downloads\New folder (2)\Bank-Api"

REM Remove .vs folders
rmdir /s /q "src\.vs" 2>nul
rmdir /s /q ".vs" 2>nul

REM Clear NuGet caches
rmdir /s /q "%LocalAppData%\NuGet\v3-cache" 2>nul
rmdir /s /q "%LocalAppData%\Temp\NuGetScratch" 2>nul

REM Clean bin and obj folders
for /d /r "src" %%d in (bin, obj) do @if exist "%%d" rmdir /s /q "%%d" 2>nul

echo.
echo Cleaning complete. Now run:
echo cd src
echo dotnet clean
echo dotnet restore
echo dotnet build
echo.
pause

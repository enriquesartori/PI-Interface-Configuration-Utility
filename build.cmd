@echo off
echo Building PI Interface Configuration Utility...
echo.

REM Check if .NET 6 SDK is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET 6 SDK is not installed or not found in PATH
    echo Please install .NET 6 SDK from https://dotnet.microsoft.com/download/dotnet/6.0
    pause
    exit /b 1
)

REM Clean previous builds
echo Cleaning previous builds...
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
if exist "publish" rmdir /s /q "publish"

REM Restore NuGet packages
echo Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore NuGet packages
    pause
    exit /b 1
)

REM Build the application
echo Building the application...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

REM Publish as standalone executable for Windows x64
echo Publishing standalone executable for Windows x64...
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish\win-x64" -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true
if %errorlevel% neq 0 (
    echo ERROR: Publish failed
    pause
    exit /b 1
)

REM Publish as standalone executable for Windows x86
echo Publishing standalone executable for Windows x86...
dotnet publish --configuration Release --runtime win-x86 --self-contained true --output "publish\win-x86" -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true
if %errorlevel% neq 0 (
    echo ERROR: Publish x86 failed
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo.
echo Standalone executables created:
echo - Windows x64: publish\win-x64\PIInterfaceConfigUtility.exe
echo - Windows x86: publish\win-x86\PIInterfaceConfigUtility.exe
echo.
echo The executables are self-contained and do not require .NET installation on target machines.
echo.
pause 
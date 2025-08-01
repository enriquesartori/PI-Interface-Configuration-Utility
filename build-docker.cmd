@echo off
echo Building PI Interface Configuration Utility using Docker...
echo.

REM Check if Docker is available
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Docker is not installed or not found in PATH
    echo Please install Docker Desktop from https://www.docker.com/products/docker-desktop
    echo.
    echo Alternative: Use GitHub Actions build instead (see ALTERNATIVE_BUILDS.md)
    pause
    exit /b 1
)

echo Docker found, building application...
echo.

REM Clean previous builds
if exist "PIInterfaceConfigUtility-x64.exe" del "PIInterfaceConfigUtility-x64.exe"
if exist "PIInterfaceConfigUtility-x86.exe" del "PIInterfaceConfigUtility-x86.exe"

REM Build Docker image
echo Building Docker image (this will download .NET 6 and compile the app)...
docker build -t pi-interface-util .
if %errorlevel% neq 0 (
    echo ERROR: Docker build failed
    pause
    exit /b 1
)

REM Extract built files
echo Extracting built executables...

REM Create temporary container
docker create --name temp-pi-util pi-interface-util
if %errorlevel% neq 0 (
    echo ERROR: Failed to create container
    pause
    exit /b 1
)

REM Copy executables
docker cp temp-pi-util:/win-x64/PIInterfaceConfigUtility.exe ./PIInterfaceConfigUtility-x64.exe
docker cp temp-pi-util:/win-x86/PIInterfaceConfigUtility.exe ./PIInterfaceConfigUtility-x86.exe

REM Cleanup
docker rm temp-pi-util
docker rmi pi-interface-util

echo.
echo Build completed successfully!
echo.
echo Executables created:
echo - PIInterfaceConfigUtility-x64.exe (Windows 64-bit)
echo - PIInterfaceConfigUtility-x86.exe (Windows 32-bit)
echo.
echo These are standalone executables that require no additional installation.
echo.
pause 
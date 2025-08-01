@echo off
echo Building PI Interface Configuration Utility (.NET Framework)...
echo.

REM Check if MSBuild is available
where msbuild >nul 2>&1
if %errorlevel% neq 0 (
    echo Trying to locate MSBuild via Visual Studio...
    
    REM Try Visual Studio 2022
    if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
        goto :build
    )
    if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
        goto :build
    )
    if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
        goto :build
    )
    
    REM Try Visual Studio 2019
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
        goto :build
    )
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
        goto :build
    )
    
    REM Try Build Tools
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
        set "MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
        goto :build
    )
    
    echo ERROR: MSBuild not found. Please install one of the following:
    echo - Visual Studio 2019/2022 (any edition)
    echo - Visual Studio Build Tools
    echo - .NET Framework Developer Pack
    echo.
    echo Download from: https://visualstudio.microsoft.com/downloads/
    pause
    exit /b 1
) else (
    set "MSBUILD=msbuild"
)

:build
echo Using MSBuild: %MSBUILD%
echo.

REM Clean previous builds
echo Cleaning previous builds...
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
if exist "publish-framework" rmdir /s /q "publish-framework"

REM Restore NuGet packages
echo Restoring NuGet packages...
nuget restore PIInterfaceConfigUtility-Framework.csproj -PackagesDirectory packages
if %errorlevel% neq 0 (
    echo NuGet restore failed. Trying with MSBuild...
    "%MSBUILD%" PIInterfaceConfigUtility-Framework.csproj /t:Restore
    if %errorlevel% neq 0 (
        echo ERROR: Failed to restore packages
        pause
        exit /b 1
    )
)

REM Build the application
echo Building the application...
"%MSBUILD%" PIInterfaceConfigUtility-Framework.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=publish-framework\
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo.
echo Executable created: publish-framework\PIInterfaceConfigUtility.exe
echo.
echo This executable requires .NET Framework 4.8 (pre-installed on Windows 10/11)
echo.
pause 
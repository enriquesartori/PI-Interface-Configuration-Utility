@echo off
echo Creating all PI Interface Configuration Utility project files...
echo.

REM Create directory structure
echo Creating folder structure...
if not exist "Models" mkdir Models
if not exist "Services" mkdir Services
if not exist "Controls" mkdir Controls
if not exist "Dialogs" mkdir Dialogs
if not exist ".github" mkdir .github
if not exist ".github\workflows" mkdir ".github\workflows"

echo ✅ Folder structure created
echo.

echo 📝 Note: This script creates the folder structure.
echo    The actual C# source files need to be recreated since they're not in this directory.
echo.
echo 🎯 RECOMMENDED APPROACH:
echo    1. Run the 'github-upload.sh' script with Git Bash
echo    2. It will upload the existing files (README.md, documentation)
echo    3. Manually add the missing C# files through GitHub web interface
echo.
echo 📁 Current directory contents:
dir
echo.
echo 🚀 To upload to GitHub:
echo    1. Install Git Bash from: https://git-scm.com/download/win
echo    2. Edit github-upload.sh and change username/email
echo    3. Run: bash github-upload.sh
echo.
pause 
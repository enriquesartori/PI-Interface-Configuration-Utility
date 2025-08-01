@echo off
echo 🔧 Pushing build fixes to GitHub...
echo.

echo 📝 Adding fixed files...
git add Models/PIPoint.cs
git add Dialogs/EditInterfaceDialog.cs  
git add Controls/LogsViewerControl.cs
git add Controls/DiagnosticsControl.cs

echo 💾 Committing fixes...
git commit -m "Fix build errors: Add missing PIPoint.cs and EditInterfaceDialog.cs, fix Timer references"

echo ⬆️ Pushing to GitHub...
git push

echo.
echo ✅ Build fixes pushed! 
echo 🔍 Check GitHub Actions in ~30 seconds:
echo    https://github.com/enriquesartori/PI-Interface-Configuration-Utility/actions
echo.
pause 
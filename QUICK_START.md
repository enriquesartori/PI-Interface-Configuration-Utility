# 🚀 Quick Start Guide (No .NET 6 Required)

Since you don't have .NET 6 installed, here's the **fastest way** to get your PI Interface Configuration Utility built:

## Method 1: GitHub Actions (Recommended)

**⏱️ Time: 5 minutes setup + 3 minutes build**

### Step 1: Create GitHub Repository
1. Go to [GitHub.com](https://github.com) and sign up (free)
2. Click "New repository"
3. Name it "PI-Interface-Utility"
4. Make it public (for free builds)
5. Click "Create repository"

### Step 2: Upload Files
1. Click "uploading an existing file"
2. Drag and drop ALL the project files
3. Write commit message: "Initial PI Interface Utility"
4. Click "Commit changes"

### Step 3: Get Your Executable
1. Go to "Actions" tab in your repository
2. Click on the build (should be running automatically)
3. Wait 2-3 minutes for completion
4. Click "Artifacts" section
5. Download "PIInterfaceConfigUtility-win-x64" or "PIInterfaceConfigUtility-win-x86"

**✅ Done! You now have a standalone .exe file!**

---

## Method 2: Docker (If Available)

**⏱️ Time: 10 minutes**

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Open PowerShell in project folder
3. Run: `.\build-docker.cmd`
4. Wait for build to complete
5. Find executables in project folder

---

## Method 3: Online Development Environment

**⏱️ Time: 15 minutes**

### Using GitHub Codespaces:
1. In your GitHub repository, click green "Code" button
2. Select "Codespaces" tab
3. Click "Create codespace on main"
4. Wait for environment to load
5. In the terminal, run:
   ```bash
   dotnet restore
   dotnet publish --configuration Release --runtime win-x64 --self-contained true --output publish -p:PublishSingleFile=true
   ```
6. Download the executable from the `publish` folder

### Using Replit:
1. Go to [Replit.com](https://replit.com)
2. Create new repl → "Import from GitHub"
3. Enter your repository URL
4. Install .NET: `curl -fsSL https://dot.net/v1/dotnet-install.sh | bash`
5. Build the project

---

## Method 4: Use Someone Else's Computer

**⏱️ Time: 30 minutes**

1. Find a computer with Visual Studio or .NET 6
2. Copy project files via USB/email/cloud
3. Run `.\build.cmd`
4. Copy executable back

**Locations to try:**
- University computer lab
- Public library with developer tools
- Friend's computer
- Work computer (if allowed)
- Internet café with development tools

---

## 📦 What You'll Get

After building with any method:
- **PIInterfaceConfigUtility.exe** (~70MB)
- **No installation required** on target machines
- **Works on any Windows 10/11** computer
- **All features included** (PI Server connection, interface management, etc.)

---

## 🆘 Still Stuck?

**Choose the easiest option for you:**

1. **Have GitHub account?** → Use GitHub Actions (Method 1)
2. **Have Docker?** → Use Docker build (Method 2)  
3. **Have fast internet?** → Use online IDE (Method 3)
4. **Have access to another PC?** → Use someone else's setup (Method 4)

**Emergency option:** Download a pre-built version from the GitHub releases page (if available).

---

## 🎯 Recommended: GitHub Actions

**Why GitHub Actions is best:**
- ✅ No local software needed
- ✅ Free for public repositories
- ✅ Builds multiple versions (x64, x86)
- ✅ Reliable and fast
- ✅ Can rebuild anytime
- ✅ Creates downloadable artifacts

**Just upload your files to GitHub and let their servers do the work!** 
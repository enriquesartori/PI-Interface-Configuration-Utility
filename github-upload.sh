#!/bin/bash

# PI Interface Configuration Utility - GitHub Upload Script
# This script uploads your project to GitHub using Git Bash

echo "🚀 PI Interface Configuration Utility - GitHub Upload"
echo "=================================================="

# Configuration - CHANGE THESE VALUES
GITHUB_USERNAME="enriquesartori"          # Replace with your GitHub username
REPO_NAME="PI-Interface-Configuration-Utility"
GITHUB_EMAIL="enriquesartori@gmail.com"         # Replace with your GitHub email

echo ""
echo "📋 Configuration:"
echo "   Username: $GITHUB_USERNAME"
echo "   Repository: $REPO_NAME"
echo "   Email: $GITHUB_EMAIL"
echo ""

# Check if we're in the right directory
if [ ! -f "PIInterfaceConfigUtility.csproj" ]; then
    echo "❌ ERROR: PIInterfaceConfigUtility.csproj not found!"
    echo "   Make sure you're in the project directory."
    echo "   Current directory: $(pwd)"
    echo ""
    echo "   If files are missing, make sure all project files are in this folder:"
    ls -la
    echo ""
    read -p "Press Enter to continue anyway, or Ctrl+C to exit..."
fi

# Verify Git is installed
if ! command -v git &> /dev/null; then
    echo "❌ ERROR: Git is not installed or not in PATH"
    echo "   Please install Git from: https://git-scm.com/download/win"
    exit 1
fi

echo "✅ Git found: $(git --version)"

# Configure Git (if not already configured)
echo ""
echo "🔧 Configuring Git..."
git config --global user.name "$GITHUB_USERNAME" 2>/dev/null || true
git config --global user.email "$GITHUB_EMAIL" 2>/dev/null || true

# Check if already a git repository
if [ -d ".git" ]; then
    echo "⚠️  Git repository already exists. Continuing..."
else
    echo "📁 Initializing Git repository..."
    git init
    if [ $? -ne 0 ]; then
        echo "❌ ERROR: Failed to initialize Git repository"
        exit 1
    fi
fi

# Create .gitignore
echo "📝 Creating .gitignore..."
cat > .gitignore << 'EOF'
# Build outputs
bin/
obj/
publish/
publish-framework/

# Visual Studio files
.vs/
*.user
*.suo
*.userosscache
*.sln.docstates

# NuGet packages
packages/
*.nupkg

# OS files
.DS_Store
Thumbs.db

# IDE files
*.swp
*.swo
*~

# Logs
*.log

# Temporary files
*.tmp
*.temp
EOF

# Show current files
echo ""
echo "📂 Files in current directory:"
ls -la

echo ""
echo "📊 Project file count:"
echo "   C# files: $(find . -name "*.cs" -type f | wc -l)"
echo "   Project files: $(find . -name "*.csproj" -type f | wc -l)"
echo "   Documentation: $(find . -name "*.md" -type f | wc -l)"

# Add all files
echo ""
echo "📦 Adding files to Git..."
git add .

if [ $? -ne 0 ]; then
    echo "❌ ERROR: Failed to add files to Git"
    exit 1
fi

# Show what's being committed
echo ""
echo "📋 Files to be committed:"
git status --short

# Commit
echo ""
echo "💾 Committing files..."
git commit -m "Initial commit: PI Interface Configuration Utility

- Complete C# Windows Forms application
- PI Server connection management
- Interface configuration and control
- PI Points management and monitoring
- Service management and diagnostics
- Configuration import/export functionality
- Real-time logging and troubleshooting
- Standalone executable with no installation required"

if [ $? -ne 0 ]; then
    echo "❌ ERROR: Failed to commit files"
    echo "   This might happen if no files were added or if Git is not configured properly"
    exit 1
fi

# Set up remote repository
echo ""
echo "🔗 Setting up remote repository..."
REPO_URL="https://github.com/$GITHUB_USERNAME/$REPO_NAME.git"
echo "   Repository URL: $REPO_URL"

# Remove existing remote if it exists
git remote remove origin 2>/dev/null || true

# Add remote
git remote add origin "$REPO_URL"

if [ $? -ne 0 ]; then
    echo "❌ ERROR: Failed to add remote repository"
    echo "   Make sure you've created the repository on GitHub first!"
    echo ""
    echo "📝 MANUAL STEPS REQUIRED:"
    echo "   1. Go to https://github.com"
    echo "   2. Click 'New repository'"
    echo "   3. Repository name: $REPO_NAME"
    echo "   4. Make it PUBLIC (required for free GitHub Actions)"
    echo "   5. DON'T initialize with README, .gitignore, or license"
    echo "   6. Click 'Create repository'"
    echo "   7. Run this script again"
    echo ""
    read -p "Press Enter after creating the repository on GitHub..."
    
    # Try again
    git remote add origin "$REPO_URL"
    if [ $? -ne 0 ]; then
        echo "❌ Still failed. Please check your GitHub username and repository name."
        exit 1
    fi
fi

# Set default branch to main
echo ""
echo "🌿 Setting default branch to main..."
git branch -M main

# Push to GitHub
echo ""
echo "⬆️  Pushing to GitHub..."
echo "   Note: You'll need to authenticate with GitHub"
echo "   Use your GitHub username and a Personal Access Token as password"
echo "   (NOT your regular password - create a token at: https://github.com/settings/tokens)"
echo ""

git push -u origin main

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ ERROR: Failed to push to GitHub"
    echo ""
    echo "🔑 AUTHENTICATION HELP:"
    echo "   1. Username: Your GitHub username"
    echo "   2. Password: Use a Personal Access Token (NOT your GitHub password)"
    echo "      - Go to: https://github.com/settings/tokens"
    echo "      - Click 'Generate new token (classic)'"
    echo "      - Select scopes: 'repo' (full repository access)"
    echo "      - Copy the token and use it as your password"
    echo ""
    echo "🔄 ALTERNATIVE: Use SSH instead of HTTPS"
    echo "   - Set up SSH keys: https://docs.github.com/en/authentication/connecting-to-github-with-ssh"
    echo ""
    echo "🌐 FALLBACK: Use GitHub web interface"
    echo "   - Upload files manually at: https://github.com/$GITHUB_USERNAME/$REPO_NAME"
    echo ""
    exit 1
fi

echo ""
echo "🎉 SUCCESS! Your project has been uploaded to GitHub!"
echo ""
echo "📊 Summary:"
echo "   ✅ Repository: https://github.com/$GITHUB_USERNAME/$REPO_NAME"
echo "   ✅ Files committed and pushed"
echo "   ✅ GitHub Actions will start building automatically"
echo ""
echo "🚀 Next Steps:"
echo "   1. Go to: https://github.com/$GITHUB_USERNAME/$REPO_NAME"
echo "   2. Click 'Actions' tab"
echo "   3. Wait for build to complete (3-5 minutes)"
echo "   4. Download executable from 'Artifacts' section"
echo ""
echo "📖 View build progress at:"
echo "   https://github.com/$GITHUB_USERNAME/$REPO_NAME/actions"
echo ""
echo "✅ Done! Your PI Interface Configuration Utility is now building on GitHub!" 